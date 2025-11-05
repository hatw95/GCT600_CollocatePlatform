using UnityEngine;
using System;

public class BaekjaHandler : MonoBehaviour
{
    public static BaekjaHandler Instance;

    public event Action<GameObject> OnBaekjaCreated;


    [Header("Fused Baekja Settings")]
    [SerializeField] private GameObject fusedBaekjaPrefab;
    [SerializeField] private GameObject spawnPoint;  // 테이블 위치 저장
    [SerializeField] private float spawnYThreshold = 1.5f;
    [SerializeField] private float fadeDuration = 2f;    // 페이드 인/아웃 지속 시간

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnFusedBaekja(GameObject baekja1, GameObject baekja2)
    {

        // 중복 호출 방지
        if (baekja1 == null || baekja2 == null) return;

        // 백자 생성 위치 조정
        Vector3 spawnPos = spawnPoint.transform.position + Vector3.up * spawnYThreshold;
        // 정해진 위치(테이블 위)에 가상 백자 초기화
        GameObject fusedBaekja = Instantiate(fusedBaekjaPrefab, spawnPos, Quaternion.identity);
        // 서서히 나타나게 하기
        FadeUtility.Instance.FadeIn(fusedBaekja, fadeDuration, 0f);
        
        OnBaekjaCreated?.Invoke(fusedBaekja);       // 가상 백자가 생성되었다는 것을 알림

        // 기존 두 백자는 서서히 사라지고 파괴
        FadeUtility.Instance.FadeOut(baekja1, fadeDuration);
        FadeUtility.Instance.FadeOut(baekja2, fadeDuration);
        Destroy(baekja1, fadeDuration); // 페이드 다 끝난 후 제거
        Destroy(baekja2, fadeDuration);;
    }
}
