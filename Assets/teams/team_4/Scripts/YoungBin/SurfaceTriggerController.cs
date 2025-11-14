using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SurfaceTriggerController : MonoBehaviour
{
    [Header("Common")]
    public string targetTag = "Marble";     // 구슬 태그
    public bool destroyOnHit = true;        // 닿으면 구슬 삭제
    public NetworkClient networkClient;     // UDP 송신(비워두면 자동탐색)

    private void Awake()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;

        if (networkClient == null)
            networkClient = FindObjectOfType<NetworkClient>(); // 안전장치
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        // 1) PC에 영상 재생 신호 전송
        if (networkClient != null)
        {
            Debug.Log("[SurfaceTrigger] Play_Hojakdo_Video");
            networkClient.SendData("Play_Hojakdo_Video");
        }

        // 2) 구슬 제거
        if (destroyOnHit)
            Destroy(other.gameObject);
    }
}
