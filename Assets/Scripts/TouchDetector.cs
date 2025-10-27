using System.Linq;
using UnityEngine;

public class TouchDetector : MonoBehaviour
{
    public OVRSkeleton skeleton;
    public enum TouchHandType { Left, Right };
    public TouchHandType handType;
    public ScreenComm screenComm;
    private Transform index1;
  
    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Touch");
        index1 = skeleton.Bones.FirstOrDefault(x => x.Id == OVRSkeleton.BoneId.XRHand_IndexTip)?.Transform;
        var indexVec = index1 != null ? (index1.position - transform.position).normalized : transform.forward;
        Physics.Raycast(transform.position, indexVec, out RaycastHit hitInfo, 0.3f, layerMask);

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + indexVec * 0.3f);
        }
        if (hitInfo.collider == null)
        {
            return;
        }
        if (hitInfo.collider.tag == "Touch")
        {
            Vector2 hitPoint2 = new Vector2(hitInfo.point.x, hitInfo.point.y);
            Transform screenTransform = hitInfo.collider.transform;
            Vector2 screenSize = new Vector2(screenTransform.localScale.x, screenTransform.localScale.y);
            Vector2 screenOrigin = new Vector2(screenTransform.position.x - screenSize.x / 2, screenTransform.position.y - screenSize.y / 2);
            Vector2 relativeHitPoint = hitPoint2 - screenOrigin;
            Vector2 normalizedHitPoint = new Vector2(relativeHitPoint.x / screenSize.x, 1.0f - relativeHitPoint.y / screenSize.y);
            Debug.Log("Touch detected at normalized coordinates: " + normalizedHitPoint);

            screenComm.SendCoordinates(normalizedHitPoint, (int)handType);
        }
    }

    
}
