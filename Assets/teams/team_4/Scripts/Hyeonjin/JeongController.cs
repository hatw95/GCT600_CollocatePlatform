using UnityEngine;
using System.Collections;

public class JeongController : MonoBehaviour
{
    [SerializeField] private GameObject jeongPrefab;
    [SerializeField] private float fadeDuration = 2f;    // 페이드 인/아웃 지속 시간
    [SerializeField] private float spawnYThreshold = 2.0f;

    [SerializeField] private TigerController tigerController;

    private void OnEnable()
    {
        if (BaekjaHandler.Instance != null)
            BaekjaHandler.Instance.OnBaekjaCreated += ShowJeongPrefab;
        else
            StartCoroutine(WaitAndSubscribe());

        JeongBehavior.OnJeongCollision += HandleJeongCollision;
    }

    private IEnumerator WaitAndSubscribe()
    {
        // Handler가 초기화될 때까지 대기
        while (BaekjaHandler.Instance == null)
            yield return null;

        BaekjaHandler.Instance.OnBaekjaCreated += ShowJeongPrefab;
    }

    private void OnDisable()
    {
        BaekjaHandler.Instance.OnBaekjaCreated -= ShowJeongPrefab;
        JeongBehavior.OnJeongCollision -= HandleJeongCollision;
    }

    private void ShowJeongPrefab(GameObject fusedBaekja)
    {
        Vector3 jeongPos = fusedBaekja.transform.position + Vector3.up * spawnYThreshold;
        GameObject jeongObj = Instantiate(jeongPrefab, jeongPos, Quaternion.identity);

        // 정 페이드인
        FadeUtility.Instance.FadeIn(jeongObj, 1f, 2f);
    }

    private void HandleJeongCollision(GameObject jeongObj, GameObject screenObj)
    {
        Debug.Log($"Jeong collided with {screenObj.name}");

        // 정이 다른 오브젝트와 충돌했을 때 처리
        // [HJ] TODO: 충돌하는 방식, 호랑이 활성화 방식 수정
        if (screenObj.name.Contains("screen1"))
        {
            FadeUtility.Instance.FadeOut(jeongObj, 1f);
            Destroy(jeongObj, 1.5f); // 페이드 아웃 후 제거

            if (tigerController != null)
            {
                Debug.Log("Activate tigerPrefab");
                // 타이거 오브젝트 활성화
                tigerController.AppearTiger();
            }
            else
            {
                Debug.LogWarning("[JeongController] TigerController not assigned!");
            }
        }
    }
    
}
