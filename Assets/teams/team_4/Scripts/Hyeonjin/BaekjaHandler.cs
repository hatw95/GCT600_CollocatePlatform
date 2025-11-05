using UnityEngine;

public class BaekjaHandler : MonoBehaviour
{
    public static BaekjaHandler Instance;

    [Header("Fused Baekja Settings")]
    [SerializeField] public GameObject fusedBaekjaPrefab;
    [SerializeField] public GameObject spawnPoint;  // 테이블 위치 저장
    [SerializeField] public float fadeDuration = 2f;    // 페이드 인/아웃 지속 시간

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnFusedBaekja(GameObject baekja1, GameObject baekja2)
    {
        
        // 중복 호출 방지
        if (baekja1 == null || baekja2 == null) return;

        // 정해진 위치(테이블 위)에 가상 백자 생성 -> 서서히 나타나게
        GameObject fusedBaekja = Instantiate(fusedBaekjaPrefab, spawnPoint.transform.position, Quaternion.identity);
        FadeUtility.Instance.FadeIn(fusedBaekja, fadeDuration, 0f);

        // 기존 두 백자는 서서히 사라지고 파괴
        FadeUtility.Instance.FadeOut(baekja1, fadeDuration);
        FadeUtility.Instance.FadeOut(baekja2, fadeDuration);

        Destroy(baekja1, fadeDuration); // 페이드 다 끝난 후 제거
        Destroy(baekja2, fadeDuration);;
    }
}
