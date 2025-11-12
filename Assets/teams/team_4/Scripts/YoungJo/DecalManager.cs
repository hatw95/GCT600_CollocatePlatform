using UnityEngine;

public class DecalManager : MonoBehaviour
{
    private ProjectionController controller;

    public void StartDecal()
    {
        controller = FindAnyObjectByType<ProjectionController>();

        if (controller == null)
        {
            Debug.LogWarning("[DecalManager] ProjectionController를 찾을 수 없습니다.");
        }

        if (controller != null)
        {
            Debug.Log("[DecalManager] ProjectionController 발견, 데칼 투사 시작.");
            controller.ProjectOnce();
            controller.PlayFadeIn(); 
        }
    }
}
