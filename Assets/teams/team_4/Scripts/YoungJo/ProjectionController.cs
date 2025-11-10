using UnityEngine;

public class ProjectionController : MonoBehaviour
{
    public Shader shader;
    public Texture2D decalTex;
    public Transform projector;
    
    private Material decalMat;
    private Renderer targetRenderer;

    void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        
        // ✅ 원본 Material은 그대로 두고, 데칼용 Material만 생성
        decalMat = new Material(shader);
        
        // 원본 Material의 BaseMap 가져오기
        Material originalMat = targetRenderer.sharedMaterial;
        Texture baseMap = originalMat.GetTexture("_BaseMap");
        Color baseColor = originalMat.GetColor("_BaseColor");
        
        Debug.Log($"BaseMap: {baseMap}, BaseColor: {baseColor}");
        
        // 데칼 Material 설정
        if (baseMap != null)
            decalMat.SetTexture("_BaseMap", baseMap);
        decalMat.SetColor("_BaseColor", baseColor);
        decalMat.SetTexture("_DecalTex", decalTex);
        
        // ✅ Material 배열에 추가 (교체 아님!)
        Material[] mats = targetRenderer.materials;
        Material[] newMats = new Material[mats.Length + 1];
        for (int i = 0; i < mats.Length; i++)
            newMats[i] = mats[i];
        newMats[mats.Length] = decalMat;
        
        targetRenderer.materials = newMats;
    }

    void Update()
    {
        if (projector == null) return;
        if (decalMat == null) return;

        Matrix4x4 view = projector.worldToLocalMatrix;
        Matrix4x4 proj = Matrix4x4.Ortho(-1, 1, -1, 1, 0.01f, 10f);

        Matrix4x4 uvTransform = Matrix4x4.identity;
        uvTransform.m00 = 0.5f; uvTransform.m03 = 0.5f;
        uvTransform.m11 = 0.5f; uvTransform.m13 = 0.5f;

        Matrix4x4 projectorMatrix = uvTransform * proj * view;
        decalMat.SetMatrix("_ProjectorMatrix", projectorMatrix);
    }
}