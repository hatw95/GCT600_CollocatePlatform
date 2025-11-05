using UnityEngine;

public class BaekjaBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BaekjaBehavior otherBaekja = other.GetComponent<BaekjaBehavior>();
        if (otherBaekja != null && otherBaekja != this)
        {
            Debug.Log($"{name} collided with {otherBaekja.name}");
            BaekjaManager.Instance.OnBaekjaCollision(gameObject, otherBaekja.gameObject);
        }
    }
}
