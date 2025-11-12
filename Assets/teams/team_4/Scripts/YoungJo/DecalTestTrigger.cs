using UnityEngine;

public class DecalTestTrigger : MonoBehaviour
{
    private ProjectionController controller;

    void Awake()
    {
        controller = FindAnyObjectByType<ProjectionController>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (controller != null)
            {
                controller.ProjectOnce();
                controller.PlayFadeIn(); 
            }
        }
    }
}
