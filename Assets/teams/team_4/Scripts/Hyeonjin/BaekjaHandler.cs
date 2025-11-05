using UnityEngine;

public class BaekjaHandler : MonoBehaviour
{
    public static BaekjaHandler Instance;

    [Header("Fused Baekja Settings")]
    [SerializeField] public GameObject fusedBaekjaPrefab;
    [SerializeField] public GameObject spawnPoint;  // 테이블 위치 저장

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnFusedBaekja(GameObject baekja1, GameObject baekja2)
    {
        
        // 중복 호출 방지
        if (baekja1 == null || baekja2 == null) return;

        // 정해진 위치(테이블 위)에 가상 백자 생성
        Instantiate(fusedBaekjaPrefab, spawnPoint.transform.position, Quaternion.identity);

        // 충돌한 두 백자 제거
        Destroy(baekja1);
        Destroy(baekja2);
    }
}
