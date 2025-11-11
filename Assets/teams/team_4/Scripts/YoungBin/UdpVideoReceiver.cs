using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

// UDP로 들어온 JSON 메시지를 파싱해 영상 재생/정지/볼륨 제어
[Serializable]
public class UdpCommand
{
    public string cmd;    // play, stop, volume 등
    public string clip;   // 파일명
    public float value;   // 볼륨, 재생위치 등
}

public class UdpVideoReceiver : MonoBehaviour
{
    [Header("UDP Settings")]
    public int listenPort = 8000;

    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public string videoFolderPath = "C:/Videos"; // 영상 파일이 있는 폴더

    private UdpClient udpClient;
    private CancellationTokenSource cts;

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        cts = new CancellationTokenSource();

        Debug.Log($"[UDP] Listening on port {listenPort}");
        _ = ReceiveLoop(cts.Token);
    }

    async Task ReceiveLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var result = await udpClient.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);

                Debug.Log($"[UDP] Received: {msg}");
                HandleMessage(msg);
            }
            catch (ObjectDisposedException) { break; }
            catch (Exception ex) { Debug.LogError(ex.Message); }
        }
    }

    void HandleMessage(string json)
    {
        try
        {
            var data = JsonUtility.FromJson<UdpCommand>(json);
            if (data == null) return;

            switch (data.cmd)
            {
                case "play":
                    PlayVideo(data.clip);
                    break;

                case "stop":
                    videoPlayer.Stop();
                    break;

                case "volume":
                    var audio = videoPlayer.GetTargetAudioSource(0);
                    if (audio != null)
                        audio.volume = Mathf.Clamp01(data.value);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[UDP] Parse Error: " + e.Message);
        }
    }

    void PlayVideo(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogWarning("[UDP] No clip name provided.");
            return;
        }

        string fullPath = System.IO.Path.Combine(videoFolderPath, clipName);
        if (!System.IO.File.Exists(fullPath))
        {
            Debug.LogWarning("[UDP] File not found: " + fullPath);
            return;
        }

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = fullPath;
        videoPlayer.Play();

        Debug.Log("[UDP] Playing video: " + fullPath);
    }

    void OnDestroy()
    {
        cts?.Cancel();
        udpClient?.Close();
        udpClient?.Dispose();
    }
}
