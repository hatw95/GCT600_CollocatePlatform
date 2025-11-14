using UnityEngine;
using System;

public class JeongBehavior : MonoBehaviour
{
    public static event Action<GameObject, GameObject> OnJeongCollision;     // 정이 어떤 오브젝트랑 충돌했는지 알려주는 이벤트

    private void OnTriggerEnter(Collider other)
    {
        // 자기 자신이랑 닿은 오브젝트를 이벤트로 보냄
        OnJeongCollision?.Invoke(gameObject, other.gameObject);
    }
}
