using UnityEngine;
using System.Collections;

public class FadeUtility : MonoBehaviour
{
    public static FadeUtility Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void FadeIn(GameObject target, float duration = 1f, float delay = 0f)
    {
        StartCoroutine(FadeRoutine(target, 0f, 1f, duration, delay, true));
    }

    public void FadeOut(GameObject target, float duration = 1f, float delay = 0f)
    {
        StartCoroutine(FadeRoutine(target, 1f, 0f, duration, delay, false));
    }

    private IEnumerator FadeRoutine(GameObject target, float startAlpha, float endAlpha, float duration, float delay, bool isFadeIn)
    {
        if (target == null) yield break;
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        // 초기 상태 설정
        SetAlpha(renderers, startAlpha);

        // 페이드 인인 경우, 딜레이 전에는 아예 렌더러 꺼버리기
        if (isFadeIn)
            SetRendererEnabled(renderers, false);

        // 딜레이 대기
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        // 페이드 시작 전에 렌더러 다시 켜기 (페이드인용)
        if (isFadeIn)
            SetRendererEnabled(renderers, true);

        // 페이드 루프
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (target == null) yield break;
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetAlpha(renderers, alpha);

            yield return null;
        }

        // 최종 알파 보정
        SetAlpha(renderers, endAlpha);

        // 페이드아웃 완료 후 완전히 숨김 처리
        if (!isFadeIn)
            SetRendererEnabled(renderers, false);
    }

    private void SetAlpha(Renderer[] renderers, float alpha)
    {
        foreach (var r in renderers)
        {
            if (r == null) continue;
            foreach (var mat in r.materials)
            {
                if (mat == null) continue;
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }
        }
    }

    private void SetRendererEnabled(Renderer[] renderers, bool enabled)
    {
        foreach (var r in renderers)
        {
            if (r != null)
                r.enabled = enabled;
        }
    }
}
