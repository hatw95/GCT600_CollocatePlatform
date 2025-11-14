using UnityEngine;

public class BaekjaBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BaekjaBehavior otherBaekja = other.GetComponent<BaekjaBehavior>();
        
        if (otherBaekja != null && otherBaekja != this)
        {
            // 한쪽만 실행 (이름이나 ID 기준)
            if (GetInstanceID() < otherBaekja.GetInstanceID())
            {
                Debug.Log($"[Collision] {name} → {otherBaekja.name} (Handled Once)");
                BaekjaManager.Instance.OnBaekjaCollision(gameObject, otherBaekja.gameObject, BaekjaManager.Instance.perfectBaekja);
            }
        }
    }

}
