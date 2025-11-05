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

    // 오브젝트 Renderer 기준으로 페이드 인
    public void FadeIn(GameObject target, float duration = 1f, float delay = 0f)
    {
        StartCoroutine(FadeRoutine(target, 0f, 1f, duration, delay));
    }

    // 오브젝트 Renderer 기준으로 페이드 아웃
    public void FadeOut(GameObject target, float duration = 1f, float delay = 0f)
    {
        StartCoroutine(FadeRoutine(target, 1f, 0f, duration, delay));
    }

    private IEnumerator FadeRoutine(GameObject target, float startAlpha, float endAlpha, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);

            foreach (var r in renderers)
            {
                foreach (var mat in r.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = alpha;
                        mat.color = c;
                    }
                }
            }
            yield return null;
        }
    }
}
