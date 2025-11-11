using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TriggerController : MonoBehaviour
{
    [Header("Trigger Target Tag")]
    public string tokenTag = "Marble";  // 감정 구슬

    [Header("Networking")]
    public NetworkClient networkClient;

    // 이미 처리했는지(중복 방지)
    private bool consumed = false;

    private void Awake()
    {
        var col = GetComponent<BoxCollider>();
        if (col == null || !col.isTrigger)
            Debug.LogWarning($"[TriggerController] BoxCollider missing or isTrigger=false on {name}");

        if (networkClient == null)
            networkClient = FindObjectOfType<NetworkClient>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;                    // 이미 한 번 처리했으면 무시
        if (!other.CompareTag(tokenTag)) return; // 'Marble' 태그만 반응

        consumed = true;

        // PC로 영상 재생 메시지 전송
        if (networkClient != null)
        {
            Debug.Log("[TriggerController] Marble entered trigger → Play_Hojakdo_Video");
            networkClient.SendData("Play_Hojakdo_Video");
        }
        else
        {
            Debug.LogError("[TriggerController] NetworkClient not assigned/found.");
        }

        // 구슬 제거 (사라지게)
        Destroy(other.gameObject);

        // 트리거 비활성화 (재트리거 방지)
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    // Exit은 사용 안 함 (넣으면 빼기 불가)
}
