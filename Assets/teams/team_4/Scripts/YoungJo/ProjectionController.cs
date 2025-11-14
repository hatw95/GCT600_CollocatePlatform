using UnityEngine;
using System.Collections;

public class ProjectionController : MonoBehaviour
{
    [Header("Required")]
    public Shader shader;
    public Texture2D decalTex;
    public Transform projector;

    [Header("Auto-Run")]
    public bool autoRun = false;
  
    private bool fadeOnStart = true;

    [Header("Fade Settings")]
    [Tooltip("등장 시간(초)")]
    public float fadeInDuration = 0.8f;
    [Tooltip("등장 곡선")]
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Material decalMat;
    private Renderer targetRenderer;
    private Coroutine fadeRoutine;

    void Start()
    {
        Debug.Log($"[ProjectionController] Start 시작 - GameObject: {gameObject.name}");
    
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("[ProjectionController] Renderer 없음!");
            enabled = false; return;
        }
        Debug.Log($"[ProjectionController] Renderer 찾음: {targetRenderer.name}");

        if (shader == null)
        {
            Debug.LogError("[ProjectionController] Shader 미지정!");
            enabled = false; return;
        }
        Debug.Log($"[ProjectionController] Shader: {shader.name}");

        if (projector == null && Camera.main != null)
            projector = Camera.main.transform;
        
        Debug.Log($"[ProjectionController] Projector: {(projector != null ? projector.name : "NULL")}");

        decalMat = new Material(shader);
        Debug.Log($"[ProjectionController] DecalMat 생성: {decalMat != null}");
    
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogWarning("[ProjectionController] Renderer 없음.");
            enabled = false; return;
        }

        if (shader == null)
        {
            Debug.LogWarning("[ProjectionController] Shader 미지정.");
            enabled = false; return;
        }

        if (projector == null && Camera.main != null)
            projector = Camera.main.transform;

        decalMat = new Material(shader);

        // 원본 Material의 BaseMap/Color (가능하면 가져오기)
        Material originalMat = targetRenderer.sharedMaterial;
        Texture baseMap = null;
        Color baseColor = Color.white;
        if (originalMat != null)
        {
            if (originalMat.HasProperty("_BaseMap"))
                baseMap = originalMat.GetTexture("_BaseMap");
            else if (originalMat.HasProperty("_MainTex"))
                baseMap = originalMat.GetTexture("_MainTex");

            if (originalMat.HasProperty("_BaseColor"))
                baseColor = originalMat.GetColor("_BaseColor");
            else if (originalMat.HasProperty("_Color"))
                baseColor = originalMat.GetColor("_Color");
        }

        if (baseMap != null) decalMat.SetTexture("_BaseMap", baseMap);
        decalMat.SetColor("_BaseColor", baseColor);
        if (decalTex != null) decalMat.SetTexture("_DecalTex", decalTex);

        
        if (decalMat.HasProperty("_DecalAlpha"))
            decalMat.SetFloat("_DecalAlpha", fadeOnStart ? 0f : 1f);

        
        var mats = targetRenderer.materials;
        var newMats = new Material[mats.Length + 1];
        for (int i = 0; i < mats.Length; i++) newMats[i] = mats[i];
        newMats[mats.Length] = decalMat;
        targetRenderer.materials = newMats;

        
        if (autoRun) StartCoroutine(IE_AutoProjectAndShow());
    }

    private IEnumerator IE_AutoProjectAndShow()
    {
        // 머티리얼 교체 직후 한 프레임 양보(드라이버 안정화를 위해)
        yield return null;

        
        ProjectOnce(); // 즉시 한 번 계산

        if (fadeOnStart) PlayFadeIn(fadeInDuration);
        else ShowInstant();
    }

    void Update()
    {
        
        if (projector == null || decalMat == null) return;

        Matrix4x4 view = projector.worldToLocalMatrix;
        Matrix4x4 proj = Matrix4x4.Ortho(-1, 1, -1, 1, 0.01f, 10f);

        Matrix4x4 uv = Matrix4x4.identity;
        uv.m00 = 0.5f; uv.m03 = 0.5f;
        uv.m11 = 0.5f; uv.m13 = 0.5f;

        Matrix4x4 projectorMatrix = uv * proj * view;
        decalMat.SetMatrix("_ProjectorMatrix", projectorMatrix);
    }

    public void ProjectOnce(float near = 0.01f, float far = 10f)
    {
        if (projector == null || decalMat == null) return;

        Matrix4x4 view = projector.worldToLocalMatrix;
        Matrix4x4 proj = Matrix4x4.Ortho(-1, 1, -1, 1, near, far);

        Matrix4x4 uv = Matrix4x4.identity;
        uv.m00 = 0.5f; uv.m03 = 0.5f;
        uv.m11 = 0.5f; uv.m13 = 0.5f;

        Matrix4x4 projectorMatrix = uv * proj * view;
        decalMat.SetMatrix("_ProjectorMatrix", projectorMatrix);
    }

    public void PlayFadeIn(float? durationOverride = null)
    {
        if (decalMat == null) return;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTo(1f, durationOverride ?? fadeInDuration));
    }

    public void HideInstant()
    {
        if (decalMat == null) return;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (decalMat.HasProperty("_DecalAlpha"))
            decalMat.SetFloat("_DecalAlpha", 0f);
    }

    public void ShowInstant()
    {
        if (decalMat == null) return;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (decalMat.HasProperty("_DecalAlpha"))
            decalMat.SetFloat("_DecalAlpha", 1f);
    }

    // --------- 내부: 페이드 ---------

    private IEnumerator FadeTo(float target, float duration)
    {
        if (!decalMat.HasProperty("_DecalAlpha")) yield break;

        float start = decalMat.GetFloat("_DecalAlpha");
        float t = 0f;

        while (t < 1f)
        {
            t += (duration > 0f ? Time.deltaTime / duration : 1f);
            float eased = fadeCurve.Evaluate(Mathf.Clamp01(t));
            float value = Mathf.LerpUnclamped(start, target, eased);
            decalMat.SetFloat("_DecalAlpha", value);
            yield return null;
        }

        decalMat.SetFloat("_DecalAlpha", target);
        fadeRoutine = null;
    }
}
