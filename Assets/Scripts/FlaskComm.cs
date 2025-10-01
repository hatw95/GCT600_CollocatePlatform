using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CoordinateSender : MonoBehaviour
{
    public string serverURL = "http://localhost:5000/receive_coordinates";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 스크린 좌표를 0-1 사이의 값으로 정규화
            float normalizedX = Input.mousePosition.x / Screen.width;
            float normalizedY = Input.mousePosition.y / Screen.height;

            // Unity의 Y축은 아래에서 위로 증가하지만, 웹의 Y축은 위에서 아래로 증가
            // 따라서 Y좌표를 웹의 기준에 맞게 뒤집어 줍니다.
            normalizedY = 1.0f - normalizedY;

            Vector2 normalizedCoords = new Vector2(normalizedX, normalizedY);

            StartCoroutine(SendNormalizedCoordinates(normalizedCoords));
        }
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
}