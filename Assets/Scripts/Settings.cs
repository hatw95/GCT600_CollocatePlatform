using System.Collections;
using System.Linq;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public OVRHand hand;           // 같은 손의 OVRHand
    public OVRSkeleton skeleton;   // 같은 손의 OVRSkeleton

    public Transform button;       // 월드 스페이스 버튼(또는 그 루트 Transform)
    public Camera xrCamera;        // CenterEyeAnchor 권장

    public float offsetAlongNormal = 0.06f;
    public float appearDotThreshold = 0.5f;
    public float smoothPos = 20f;
    public float smoothRot = 20f;
    public bool requireOpenPalm = true;
    public float openPalmPinchMax = 0.2f;

    Transform wrist, index1, pinky1;
    bool bonesReady;

    IEnumerator Start()
    {
        if (!xrCamera) xrCamera = Camera.main;

        // 스켈레톤 준비될 때까지 대기
        while (skeleton && (!skeleton.IsInitialized || skeleton.Bones == null || skeleton.Bones.Count == 0))
            yield return null;

        CacheBones();
        bonesReady = wrist && index1 && pinky1;
        Toggle(false);
    }

    void CacheBones()
    {
        wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);
        index1 = GetBone(OVRSkeleton.BoneId.Hand_Index1);
        pinky1 = GetBone(OVRSkeleton.BoneId.Hand_Pinky1);
    }

    Transform GetBone(OVRSkeleton.BoneId id)
    {
        var b = skeleton.Bones.FirstOrDefault(x => x.Id == id);
        return b != null ? b.Transform : null;
    }

    void Update()
    {
        if (!hand || !skeleton || !button || !xrCamera || !bonesReady)
        {
            Toggle(false);
            return;
        }

        // 트래킹 신뢰도 체크
        bool tracked = hand.IsTracked &&
                       hand.HandConfidence == OVRHand.TrackingConfidence.High &&
                       skeleton.IsDataValid;
        if (!tracked) { Toggle(false); return; }

        // 손을 펴야만 표시(옵션)
        if (requireOpenPalm)
        {
            bool anyPinch =
                hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > openPalmPinchMax ||
                hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > openPalmPinchMax ||
                hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > openPalmPinchMax ||
                hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) > openPalmPinchMax;
            if (anyPinch) { Toggle(false); return; }
        }

        // 손바닥 중심 & 법선
        Vector3 pw = wrist.position;
        Vector3 pi = index1.position;
        Vector3 pp = pinky1.position;

        Vector3 palmCenter = (pw + pi + pp) / 3f;
        Vector3 v1 = (pi - pw).normalized;
        Vector3 v2 = (pp - pw).normalized;
        Vector3 palmNormal = Vector3.Normalize(Vector3.Cross(v1, v2));
        // 왼손/오른손에 따라 노멀 뒤집힘 보정
        OVRSkeleton.SkeletonType handType = skeleton.GetSkeletonType();
        if (handType == OVRSkeleton.SkeletonType.HandLeft)
            palmNormal = -palmNormal;

        // 손바닥이 카메라(머리) 쪽을 보는지
        Vector3 toHead = (xrCamera.transform.position - palmCenter).normalized;
        float facing = Vector3.Dot(-palmNormal, toHead);
        bool palmFacingUser = facing > appearDotThreshold;

        Toggle(palmFacingUser);
        if (!palmFacingUser) return;

        // 버튼 위치/회전 (스무딩)
        Vector3 targetPos = palmCenter + palmNormal * offsetAlongNormal;
        float posLerp = 1f - Mathf.Exp(-smoothPos * Time.deltaTime);
        float rotLerp = 1f - Mathf.Exp(-smoothRot * Time.deltaTime);

        button.position = Vector3.Lerp(button.position, targetPos, posLerp);
        Quaternion lookAtUser =
            Quaternion.LookRotation(button.position - xrCamera.transform.position, xrCamera.transform.up);
        button.rotation = Quaternion.Slerp(button.rotation, lookAtUser, rotLerp);
    }

    void Toggle(bool on)
    {
        if (button && button.gameObject.activeSelf != on)
            button.gameObject.SetActive(on);
    }
}
