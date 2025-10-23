using Meta.XR.MRUtilityKit;
using Meta.XR.MRUtilityKit.BuildingBlocks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PanelPlacement : MonoBehaviour
{
    public GameObject effectMesh;
    [SerializeField] private Transform screen;
    [SerializeField] private OVRInput.RawAxis2D _scaleAxis = OVRInput.RawAxis2D.RThumbstick;
    public Transform rayStartPoint;
    public float rayLength = 5;
    private LabelFilter _labelFilter;
    private PlaceWithAnchor _placeWithAnchor;
    private EffectMesh _effectMesh;
    private bool _isPlacing = false;

    private void Awake()
    {
        _placeWithAnchor = GetComponent<PlaceWithAnchor>();
        if (_placeWithAnchor == null)
        {
            Debug.LogError("SetPanel requires PlaceWithAnchor component on the same GameObject.");
        }
        _effectMesh = effectMesh.GetComponent<EffectMesh>();
        _effectMesh.HideMesh = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void SetScreen(out Pose surfacePose)
    {
        surfacePose = new Pose(screen.position, screen.rotation);
    }

    public bool IsPlacing()
    {
        return _isPlacing;
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
        _isPlacing = true;
        var rb = screen.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);

            MRUKRoom room = MRUK.Instance.GetCurrentRoom();
            _labelFilter = new LabelFilter();
            bool hadHit = room.Raycast(ray, rayLength, _labelFilter, out RaycastHit hit, out MRUKAnchor anchor);

            if (hadHit && !_isPlacing)
            {
                if (_effectMesh.HideMesh)
                    _effectMesh.HideMesh = false;
                screen.position = hit.point;
                screen.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);

                const float scaleSpeed = 1.5f;
                var screenScaleX = screen.localScale.x;
                var screenScaleY = screen.localScale.y;
                screenScaleX *= 1f + OVRInput.Get(_scaleAxis).x * scaleSpeed * Time.deltaTime;
                screenScaleX = Mathf.Clamp(screenScaleX, 0.2f, 10.0f);
                screenScaleY *= 1f + OVRInput.Get(_scaleAxis).y * scaleSpeed * Time.deltaTime;
                screenScaleY = Mathf.Clamp(screenScaleY, 0.2f, 10.0f);
                screen.localScale = new Vector3(screenScaleX, screenScaleY, 0.005f);
            }

            if (OVRInput.GetDown(OVRInput.Button.One) && !_isPlacing)
            {
                OnSetPanel();
                _effectMesh.HideMesh = true;
            }
        }
    }

    
}
