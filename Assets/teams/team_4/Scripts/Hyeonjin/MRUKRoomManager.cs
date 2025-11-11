using UnityEngine;
using Meta.XR.MRUtilityKit;
using System;

public class MRUKManager : MonoBehaviour
{
    public static MRUKManager Instance { get; private set; }

    public bool IsReady { get; private set; } = false;
    public MRUKRoom CurrentRoom { get; private set; }

    public event Action<MRUKRoom> OnRoomReady;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeMRUK();
    }

    private void InitializeMRUK()
    {
        // MRUK 인스턴스 존재 확인
        var mruk = MRUK.Instance;
        if (mruk == null)
        {
            Debug.LogWarning("[MRUKManager] MRUK instance not found in scene. Retrying...");
            InvokeRepeating(nameof(CheckForMRUK), 0.5f, 1f);
            return;
        }

        // MRUK가 이미 초기화된 상태면 바로 실행
        if (mruk.IsInitialized)
        {
            Debug.Log("[MRUKManager] MRUK already initialized.");
            OnSceneLoaded();
        }
        else
        {
            // 아직 준비 안된 경우 SceneLoadedEvent 구독
            Debug.Log("[MRUKManager] Waiting for MRUK.SceneLoadedEvent...");
            mruk.SceneLoadedEvent.AddListener(OnSceneLoaded);
        }
    }

    private void CheckForMRUK()
    {
        if (MRUK.Instance != null)
        {
            CancelInvoke(nameof(CheckForMRUK));
            InitializeMRUK();
        }
    }

    private void OnSceneLoaded()
    {
        var mruk = MRUK.Instance;
        if (mruk == null)
        {
            Debug.LogError("[MRUKManager] SceneLoadedEvent fired but MRUK.Instance is null.");
            return;
        }

        // 현재 Room 가져오기
        CurrentRoom = mruk.GetCurrentRoom();

        if (CurrentRoom != null)
        {
            IsReady = true;
            Debug.Log($"[MRUKManager] MRUK Room Ready: {CurrentRoom.name}");
            OnRoomReady?.Invoke(CurrentRoom);
        }
        else
        {
            Debug.LogWarning("[MRUKManager] MRUK initialized but no active room found.");
        }
    }
}
