using UnityEngine;
using System;
using System.Collections;
using Meta.XR.MRUtilityKit;

public class BaekjaHandler : MonoBehaviour
{
    public static BaekjaHandler Instance;
    public event Action<GameObject> OnBaekjaCreated;

    [Header("Initial Baekja Settings")]
    [SerializeField] private GameObject baekjaAPrefab;
    [SerializeField] private GameObject baekjaBPrefab;
    [SerializeField] private float spacing = 0.25f;    // 두 백자 간 간격

    [Header("Fused Baekja Settings")]
    [SerializeField] private GameObject fusedBaekjaPrefab;
    
    [SerializeField] private float spawnYThreshold = 1.5f;
    [SerializeField] private float fadeDuration = 2f;    // 페이드 인/아웃 지속 시간
    private GameObject spawnPoint;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // MRUKManager를 통해 Room 상태 확인
        if (MRUKManager.Instance != null)
        {
            if (MRUKManager.Instance.IsReady)
                AssignTableAnchor(MRUKManager.Instance.CurrentRoom);
            else
                MRUKManager.Instance.OnRoomReady += AssignTableAnchor;
        }
        else
        {
            Debug.LogError("[BaekjaHandler] MRUKManager instance not found in scene.");
        }
    }

    private void AssignTableAnchor(MRUKRoom room)
    {
        if (room == null)
        {
            Debug.LogWarning("[BaekjaHandler] No MRUKRoom provided.");
            return;
        }

        foreach (var anchor in room.Anchors)
        {
            if (anchor.Label == MRUKAnchor.SceneLabels.TABLE)
            {
                spawnPoint = anchor.gameObject;
                Debug.Log($"[BaekjaHandler] Found TABLE anchor → {spawnPoint.name}");
                break;
            }
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("[BaekjaHandler] TABLE anchor not found in current MRUK room.");
        }

        StartCoroutine(WaitAndSpawnInitialBaekjas());
    }

    IEnumerator WaitAndSpawnInitialBaekjas()
    {
        // 최소 2프레임 기다리기
        yield return null;
        yield return null;

        // Renderer나 Collider 로드될 때까지 최대 1초(60프레임) 대기
        Renderer rend = null;
        Collider col = null;
        int frameCount = 0;
        while (rend == null && col == null && frameCount < 60)
        {
            rend = spawnPoint.GetComponentInChildren<Renderer>(true);
            col = spawnPoint.GetComponentInChildren<Collider>(true);
            frameCount++;
            yield return null;
        }

        if (rend == null && col == null)
        {
            Debug.LogWarning("[BaekjaHandler] TABLE prefab still has no Renderer or Collider after waiting.");
            yield break;
        }

        Debug.Log($"[BaekjaHandler] TABLE mesh ready after {frameCount} frames.");
        SpawnInitialBaekjas();
    }

    
    private void SpawnInitialBaekjas()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("[BaekjaHandler] TABLE anchor not found.");
            return;
        }

        Renderer rend = spawnPoint.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("[BaekjaHandler] TABLE has no Renderer.");
            return;
        }

        // 테이블 표면과 중심 계산
        Bounds bounds = rend.bounds;
        float topY = bounds.center.y + bounds.extents.y;
        Vector3 center = bounds.center;
        Vector3 right = spawnPoint.transform.right.normalized;

        // 중앙 기준 좌우로 배치
        Vector3 posA = center - right * (spacing * 0.5f);
        Vector3 posB = center + right * (spacing * 0.5f);
        posA.y = posB.y = topY + spawnYThreshold;

        // 인스펙터에서 지정된 프리팹으로 스폰
        if (baekjaAPrefab == null || baekjaBPrefab == null)
        {
            Debug.LogError("[BaekjaHandler] Please assign both Baekja prefabs in the Inspector!");
            return;
        }

        GameObject baekjaA = Instantiate(baekjaAPrefab, posA, Quaternion.identity);
        GameObject baekjaB = Instantiate(baekjaBPrefab, posB, Quaternion.identity);

        Debug.Log($"[BaekjaHandler] Spawned Baekja A at {posA}, Baekja B at {posB}");
    }

    public void SpawnFusedBaekja(GameObject baekja1, GameObject baekja2, GameObject perfectBaekja)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("[BaekjaHandler] SpawnPoint not assigned (TABLE anchor missing).");
            return;
        }

        // 중복 호출 방지
        if (baekja1 == null || baekja2 == null) return;


        // Renderer를 기준으로 높이 계산
        Renderer rend = spawnPoint.GetComponentInChildren<Renderer>();      // child에서 렌더러 찾기
        if (rend == null)
        {
            Debug.LogWarning("[BaekjaHandler] SpawnPoint에 Renderer가 없습니다.");
            return;
        }

        float topY = rend.bounds.center.y + rend.bounds.extents.y;
        Vector3 spawnPos = new Vector3(
            spawnPoint.transform.position.x,
            topY,
            spawnPoint.transform.position.z
        );


        // 정해진 위치(테이블 위)에 가상 백자 초기화
        GameObject fusedBaekja = Instantiate(
            fusedBaekjaPrefab,
            spawnPos,
            fusedBaekjaPrefab.transform.rotation
        );

        // 서서히 나타나게 하기
        FadeUtility.Instance.FadeIn(fusedBaekja, fadeDuration, 0f);

        OnBaekjaCreated?.Invoke(fusedBaekja);       // 가상 백자가 생성되었다는 것을 알림
        
        if (perfectBaekja != null)
        {
            perfectBaekja.transform.position = spawnPos;
            perfectBaekja.transform.rotation = fusedBaekjaPrefab.transform.rotation;
            //perfectBaekja.SetActive(true);
            Debug.Log($"[BaekjaHandler] PerfectBaekja 활성화 및 위치 설정: {spawnPos}");
        }
        else
        {
            Debug.LogWarning("[BaekjaHandler] PerfectBaekja가 할당되지 않았습니다.");
        }

        // 기존 두 백자는 서서히 사라지고 파괴
        FadeUtility.Instance.FadeOut(baekja1, fadeDuration);
        FadeUtility.Instance.FadeOut(baekja2, fadeDuration);
        Destroy(baekja1, fadeDuration); // 페이드 다 끝난 후 제거
        Destroy(baekja2, fadeDuration); ;
    }
    
    // Gizmo로 테이블 윗면 표시 (Scene 뷰에서 확인 가능)
    private void OnDrawGizmos()
    {
        if (spawnPoint == null) return;

        Renderer rend = spawnPoint.GetComponentInChildren<Renderer>();
        if (rend == null) return;

        // 테이블의 윗면 중앙 좌표 계산
        float topY = rend.bounds.center.y + rend.bounds.extents.y;
        Vector3 topPos = new Vector3(
            spawnPoint.transform.position.x,
            topY,
            spawnPoint.transform.position.z
        );

        // Gizmo 그리기
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(topPos, 0.03f);
    }
}
