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
            // ��ũ�� ��ǥ�� 0-1 ������ ������ ����ȭ
            float normalizedX = Input.mousePosition.x / Screen.width;
            float normalizedY = Input.mousePosition.y / Screen.height;

            // Unity�� Y���� �Ʒ����� ���� ����������, ���� Y���� ������ �Ʒ��� ����
            // ���� Y��ǥ�� ���� ���ؿ� �°� ������ �ݴϴ�.
            normalizedY = 1.0f - normalizedY;

            Vector2 normalizedCoords = new Vector2(normalizedX, normalizedY);

            StartCoroutine(SendNormalizedCoordinates(normalizedCoords));
        }
    }

    IEnumerator SendNormalizedCoordinates(Vector2 coords)
    {
        // JSON ������ ����
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