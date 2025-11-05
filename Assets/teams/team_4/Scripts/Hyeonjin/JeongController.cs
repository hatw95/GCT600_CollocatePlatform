using UnityEngine;
using System.Collections;

public class JeongController : MonoBehaviour
{
    [SerializeField] private GameObject jeongPrefab;
    [SerializeField] private float fadeDuration = 2f;    // 페이드 인/아웃 지속 시간

    private void OnEnable()
    {
        if (BaekjaHandler.Instance != null)
            BaekjaHandler.Instance.OnBaekjaCreated += ShowJeongPrefab;
        else
            StartCoroutine(WaitAndSubscribe());
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
    }

    private void ShowJeongPrefab(GameObject fusedBaekja)
    {
        Vector3 jeongPos = fusedBaekja.transform.position + Vector3.up * 1.5f;
        GameObject jeongObj = Instantiate(jeongPrefab, jeongPos, Quaternion.identity);

        // 정 페이드인
        FadeUtility.Instance.FadeIn(jeongObj, 1f, 2f);
    }
}
