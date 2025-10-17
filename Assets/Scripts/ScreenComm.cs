using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScreenComm : MonoBehaviour
{
    public string serverURL = "ws://localhost:5000/ws";
    public TMP_Text currIP;
    private ClientWebSocket ws;
    private Uri serverUri;
    private CancellationTokenSource cts;
    private int buttonIndex = -1;
    private bool isFlask = true;

    public void ToggleSetFlask(bool toggle)
    {
        isFlask = toggle;
        ConnectWebSocket();
    }

    async void ConnectWebSocket()
    {
        ws = new ClientWebSocket();
        cts = new CancellationTokenSource();

        serverUri = isFlask ? new Uri(serverURL) : new Uri(serverURL.Replace("/ws", ""));
        await ws.ConnectAsync(serverUri, cts.Token);

        _ = Task.Run(ReceiveLoop);
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
                if (msg.Contains("Received button index:"))
                {
                    buttonIndex = int.Parse(msg.Split(':')[1].Trim());
                    Debug.Log("Received button index: " + buttonIndex);
                }
                else
                {
                    buttonIndex = -1;
                }
            }
        }
    }

    public int GetButtonIndex()
    {
        return buttonIndex;
    }

    public async void SendCoordinates(Vector2 coords, int tag)
    {
        if (ws == null || ws.State != WebSocketState.Open) return;

        string json = "{\"x\":" + coords.x + ", \"y\":" + coords.y + ", \"tag\":" + tag + "}";
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));

        await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cts.Token);
        Debug.Log("Sent: " + json);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    public void SetServerURL(TMP_InputField field)
    {
        Debug.Log("Setting server URL to: " + field.text);
        serverURL = "ws://" + field.text + ":5000/ws";
        ConnectWebSocket();
        currIP.text = "Current IP: " + field.text;
        //Save serverURL to PlayerPrefs
        PlayerPrefs.SetString("serverURL", serverURL);
        PlayerPrefs.Save();
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
