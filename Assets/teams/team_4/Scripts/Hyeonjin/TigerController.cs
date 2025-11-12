using UnityEngine;

public class TigerController : MonoBehaviour
{
    [Header("Tiger Settings")]
    [SerializeField] private GameObject tigerObject;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Movement Settings")]
    [SerializeField] private Transform targetPoint;   // 인스펙터에서 Empty 오브젝트 지정
    [SerializeField] private float moveSpeed = 1.5f;  // 호랑이 이동 속도 (m/s)
    [SerializeField] private float stopDistance = 0.3f; // 도착 판정 거리

    private Animator tigerAnimator;
    private bool isWalking = false;

    private void Awake()
    {
        if (tigerObject == null)
            tigerObject = this.gameObject;

        tigerAnimator = tigerObject.GetComponent<Animator>();
        
        if (tigerAnimator == null)
        {
            Debug.LogWarning("[TigerController] No Animator component found on the tiger object.");
        }
    }

    private void Start()
    {
        // 초기에는 비활성 상태
        tigerObject.SetActive(false);
    }

    private void Update()
    {
        // // 스페이스바 누르면 AppearTiger 실행
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     AppearTiger();
        // }  
        if (Input.GetKeyDown(KeyCode.A))
        {
            tigerAnimator.SetTrigger("Idle");
            FadeUtility.Instance?.FadeOut(tigerObject, fadeDuration, 0f);

        }

        if (targetPoint != null)
        {
            AnimatorStateInfo stateInfo = tigerAnimator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfos = tigerAnimator.GetCurrentAnimatorClipInfo(0);
            if (clipInfos.Length > 0)
            {
                Debug.Log("[TigerController] 현재 재생 중인 애니메이션: " + clipInfos[0].clip.name);
            }

            // Walk 상태일 때만 이동하도록 제한
            if (stateInfo.IsName("End Roarning") && !tigerAnimator.IsInTransition(0))
            {
                MoveToTarget();
            }
        }
    }

    public void AppearTiger()
    {
        if (!tigerObject.activeSelf)
        {
            tigerObject.SetActive(true);
            FadeUtility.Instance?.FadeIn(tigerObject, fadeDuration, 0f);
        }
    }

    private void MoveToTarget()
    {
        tigerAnimator.SetBool("isWalking", true);
        Vector3 targetPos = targetPoint.position;
        Vector3 moveDir = (targetPos - tigerObject.transform.position);
        
        moveDir.y = 0; // 수평 이동만
        float distance = moveDir.magnitude;

        if (distance > stopDistance)
        {
            // 바라보는 방향 회전
            Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized);
            tigerObject.transform.rotation = Quaternion.Slerp(tigerObject.transform.rotation, targetRot, Time.deltaTime * 5f);

            // 앞으로 이동
            tigerObject.transform.position += tigerObject.transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 도착
            isWalking = false;
            Debug.Log("[TigerController] Tiger has reached the target point.");
            tigerAnimator.SetBool("isWalking", false);
            tigerAnimator.SetTrigger("Idle");
        }
    }
}