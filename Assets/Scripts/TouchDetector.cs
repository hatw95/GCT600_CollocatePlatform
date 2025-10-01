using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro ;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TouchDetector : MonoBehaviour
{
    //public GameObject explosionPrefab;
    //public PanelPlacement _panelPlacement;
    public string serverURL = "ws://localhost:5000/ws";
    public TMP_Text currIP;
    private MeshRenderer meshRenderer;
    public OVRSkeleton skeleton;
    private Transform index1;

    private ClientWebSocket ws;
    private Uri serverUri;
    private CancellationTokenSource cts;
    private bool isFlask = true;

    public void ToggleVisualize()
    {
        meshRenderer.enabled = !meshRenderer.enabled;
    }

    public void ToggleSetFlask(bool toggle)
    {
        isFlask = toggle;
        ConnectWebSocket();
    }

    async void ConnectWebSocket()
    {
        ws = new ClientWebSocket();
        cts = new CancellationTokenSource();

        serverUri =  isFlask ? new Uri(serverURL) : new Uri(serverURL.Replace("/ws", ""));
        await ws.ConnectAsync(serverUri, cts.Token);

        _ = Task.Run(ReceiveLoop);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //check if serverURL is saved in PlayerPrefs
        if (PlayerPrefs.HasKey("serverURL"))
        {
            serverURL = PlayerPrefs.GetString("serverURL", serverURL);
            currIP.text = "Current IP: " + serverURL.Replace("ws://", "").Replace(":5000/ws", "");
            Debug.Log("Loaded server URL from PlayerPrefs: " + serverURL);
        }
        else
        {
            currIP.text = "Current IP: localhost";
        }
        ConnectWebSocket();
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log("Server says: " + msg);
            }
        }
    }

    public async void SendCoordinates(Vector2 coords)
    {
        if (ws == null || ws.State != WebSocketState.Open) return;

        string json = "{\"x\":" + coords.x + ", \"y\":" + coords.y + "}";
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));

        await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cts.Token);
        Debug.Log("Sent: " + json);
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Touch");
        index1 = skeleton.Bones.FirstOrDefault(x => x.Id == OVRSkeleton.BoneId.XRHand_IndexTip)?.Transform;
        var indexVec = index1 != null ? (index1.position - transform.position).normalized : transform.forward;
        Physics.Raycast(transform.position, indexVec, out RaycastHit hitInfo, 0.2f, layerMask);
//#if Unity_EDITOR
//        Debug.DrawRay(transform.position, transform.forward * 0.05f, Color.red);
//#endif
        //Draw ray using line renderer
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + indexVec * 0.2f);
        }
        if (hitInfo.collider == null) return;
        if (hitInfo.collider.tag == "Touch")
        {
            Vector2 hitPoint2 = new Vector2(hitInfo.point.x, hitInfo.point.y);
            Transform screenTransform = hitInfo.collider.transform;
            Vector2 screenSize = new Vector2(screenTransform.localScale.x, screenTransform.localScale.y);
            Vector2 screenOrigin = new Vector2(screenTransform.position.x - screenSize.x / 2, screenTransform.position.y - screenSize.y / 2);
            Vector2 relativeHitPoint = hitPoint2 - screenOrigin;
            Vector2 normalizedHitPoint = new Vector2(relativeHitPoint.x / screenSize.x, 1.0f - relativeHitPoint.y / screenSize.y);
            Debug.Log("Touch detected at normalized coordinates: " + normalizedHitPoint);

            //StartCoroutine(SendNormalizedCoordinates(normalizedHitPoint));
            SendCoordinates(normalizedHitPoint);
        }
    }

    public void SetServerURL(TMP_InputField field)
    {
        Debug.Log("Setting server URL to: " + field.text);
        serverURL = "ws://" + field.text+ ":5000/ws";
        ConnectWebSocket();
        currIP.text = "Current IP: " + field.text;
        //Save serverURL to PlayerPrefs
        PlayerPrefs.SetString("serverURL", serverURL);
        PlayerPrefs.Save();
    }

    IEnumerator SendNormalizedCoordinates(Vector2 coords)
    {
        // JSON 데이터 생성
        string json = "{\"x\":" + coords.x + ", \"y\":" + coords.y + "}";

        UnityWebRequest request = new UnityWebRequest(serverURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Normalized coordinates sent successfully!");
        }
    }

    private async void OnApplicationQuit()
    {
        if (ws != null)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            ws.Dispose();
        }
    }
}
