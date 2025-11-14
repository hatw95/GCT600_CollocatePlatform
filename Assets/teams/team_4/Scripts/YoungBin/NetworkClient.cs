using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class NetworkClient : MonoBehaviour
{
    [Header("Network Settings")]
    public string serverIp = "192.168.0.18"; //"192.168.0.151";
    public int serverPort = 5000;

    private UdpClient udpClient;
    
    void Start()
    {
        udpClient = new UdpClient();
        Debug.Log("[Network Client] UDP Client initialized");
    }

    void OnApplicationQuit()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            Debug.Log("[Network Client] UDP Client stopped.");
        }
    }

    public void SendData(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            udpClient.Send(data, data.Length, serverEndPoint);

            Debug.Log($"[Network Client] Sent message to Server({serverIp}:{serverPort}): {message}");
        }
        catch (SocketException e)
        {
            Debug.LogError($"[Network Client] Socket Exception: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Network Client] Error sending data: {e.Message}");
        }
    }

    private void Update()
    {
        // 오른쪽 A 버튼 누르면 영상 재생
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Debug.Log("[Client] A down (RTouch)");
            SendData("Play_Hojakdo_Video");
        }
       // 왼쪽 X 버튼 누르면 이미지 복귀
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            Debug.Log("[Client] X down (LTouch)");
            SendData("Show_Hojakdo_Image");
        }
    }
}
