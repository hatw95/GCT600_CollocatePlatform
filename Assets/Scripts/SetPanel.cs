using Meta.XR.MRUtilityKit.BuildingBlocks;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SetPanel : MonoBehaviour
{
    [SerializeField] private Transform screen;
    private PlaceWithAnchor _placeWithAnchor;

    private void Awake()
    {
        _placeWithAnchor = GetComponent<PlaceWithAnchor>();
        if (_placeWithAnchor == null)
        {
            Debug.LogError("SetPanel requires PlaceWithAnchor component on the same GameObject.");
        }
    }

    private void SetScreen(out Pose surfacePose)
    {
        surfacePose = new Pose(screen.position, screen.rotation);
    }    

    public void OnSetPanel()
    {
        if (_placeWithAnchor == null)
        {
            Debug.LogError("PlaceWithAnchor component is missing.");
            return;
        }
        SetScreen(out Pose surfacePose);
        _placeWithAnchor.RequestMove(surfacePose);
    }
}
