using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class DisplayManager : MonoBehaviour
{
    [Header("Hojakdo Assets")]
    public Texture2D hojakdoImage;      // 호작도 정적 이미지
    public RenderTexture videoRT;       // VideoPlayer 출력용 RenderTexture
    public VideoPlayer videoPlayer;     // 영상 재생 컴포넌트(클립/URL 연결)

    [Header("UI (Separate RawImages)")]
    public RawImage hojakdoImageRaw;    // 이미지 전용 RawImage
    public RawImage hojakdoVideoRaw;    // 영상 전용 RawImage (texture = videoRT)

    [Header("Aspect (Optional)")]
    public AspectRatioFitter imageFitter; // 이미지용 Fitter(선택)
    public AspectRatioFitter videoFitter; // 영상용 Fitter(선택)
    public bool imageFill = false;        // true=꽉채우기(Envelope), false=전체보이기(Fit)
    public bool videoFill = true;         // 영상은 기본 꽉채우기 추천
    public bool autoSetAspect = true;     // 전환 시 비율 자동 반영

    [Header("Options")]
    public bool loopVideo = true;         // 영상 반복 재생 여부

    // 내부 상태
    private bool isVideo = false;

    private void Awake()
    {
        // 누락 대비: VideoPlayer → RenderTexture 연결 보장
        if (videoPlayer != null && videoRT != null)
            videoPlayer.targetTexture = videoRT;

        // RawImage에 텍스처 연결 보장
        if (hojakdoImageRaw != null && hojakdoImage != null)
            hojakdoImageRaw.texture = hojakdoImage;
        if (hojakdoVideoRaw != null && videoRT != null)
            hojakdoVideoRaw.texture = videoRT;

        if (videoPlayer != null) videoPlayer.isLooping = loopVideo;

        // 초기 상태: 이미지 모드
        ToImageMode(resetVideo: true);
    }

    private void Update()
    {
        // 데모용 키 입력 (원하면 제거 가능)
        if (Input.GetKeyDown(KeyCode.Space))  ChangeImage("Toggle_Hojakdo");
        else if (Input.GetKeyDown(KeyCode.A)) ChangeImage("Show_Hojakdo_Image");
        else if (Input.GetKeyDown(KeyCode.B)) ChangeImage("Play_Hojakdo_Video");
        else if (Input.GetKeyDown(KeyCode.C)) ChangeImage("StopVideo");
        else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public void ChangeImage(string message)
    {
        switch (message)
        {
            case "Toggle_Hojakdo":
                if (isVideo) ToImageMode(resetVideo: true);
                else         ToVideoMode();
                break;

            case "Show_Hojakdo_Image":
                ToImageMode(resetVideo: true);
                break;

            case "Play_Hojakdo_Video":
                ToVideoMode();
                break;

            case "StopVideo":
                ToImageMode(resetVideo: true);
                break;

            default:
                Debug.Log($"[DisplayManager] Unknown message: {message}");
                break;
        }
    }

    // ===== 내부 유틸 =====

    private void ToImageMode(bool resetVideo)
    {
        isVideo = false;

        // 표시 전환
        if (hojakdoImageRaw != null) hojakdoImageRaw.gameObject.SetActive(true);
        if (hojakdoVideoRaw != null) hojakdoVideoRaw.gameObject.SetActive(false);

        // 비율 자동 적용 (선택)
        if (autoSetAspect && hojakdoImage != null && imageFitter != null)
        {
            imageFitter.aspectMode = imageFill
                ? AspectRatioFitter.AspectMode.EnvelopeParent   // 꽉 채우기(크롭 허용)
                : AspectRatioFitter.AspectMode.FitInParent;     // 전부 보이기(여백 허용)
            imageFitter.aspectRatio = (float)hojakdoImage.width / hojakdoImage.height;
        }

        // 영상 정지/리셋
        if (videoPlayer != null)
        {
            if (resetVideo) videoPlayer.time = 0;
            videoPlayer.Pause();
        }
    }

    private void ToVideoMode()
    {
        isVideo = true;

        // 표시 전환
        if (hojakdoImageRaw != null) hojakdoImageRaw.gameObject.SetActive(false);
        if (hojakdoVideoRaw != null) hojakdoVideoRaw.gameObject.SetActive(true);

        // 비율 자동 적용 (선택)
        if (autoSetAspect && videoRT != null && videoFitter != null)
        {
            videoFitter.aspectMode = videoFill
                ? AspectRatioFitter.AspectMode.EnvelopeParent   // 꽉 채우기
                : AspectRatioFitter.AspectMode.FitInParent;     // 전부 보이기
            videoFitter.aspectRatio = (float)videoRT.width / videoRT.height; // 1:1이면 1
        }

        // 재생
        if (videoPlayer != null) videoPlayer.Play();
    }
}
