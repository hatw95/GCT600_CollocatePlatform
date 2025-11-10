using UnityEngine;

public class ProjectionController : MonoBehaviour
{
    public Shader shader;
    public Texture2D decalTex;
    public Transform projector;

    private Material mat;

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        mat = renderer.material;            
        mat.shader = shader;                
        mat.SetTexture("_DecalTex", decalTex);
    }

    void Update()
    {
        if (projector == null) return;
        if (mat == null) return;

        Matrix4x4 view = projector.worldToLocalMatrix;
        Matrix4x4 proj = Matrix4x4.Ortho(-1, 1, -1, 1, 0.01f, 10f);

        Matrix4x4 uvTransform = Matrix4x4.identity;
        uvTransform.m00 = 0.5f; uvTransform.m03 = 0.5f;
        uvTransform.m11 = 0.5f; uvTransform.m13 = 0.5f;

        Matrix4x4 projectorMatrix = uvTransform * proj * view;
        mat.SetMatrix("_ProjectorMatrix", projectorMatrix);
    }
}
