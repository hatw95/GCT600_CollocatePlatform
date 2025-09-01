using UnityEngine;

public class FollowController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
    }
}
