using UnityEngine;

public class TigerController : MonoBehaviour
{
    [Header("Tiger Settings")]
    [SerializeField] private GameObject tigerObject;
    [SerializeField] private float fadeDuration = 2f;

    private Animator tigerAnimator;

    private void Awake()
    {
        if (tigerObject == null)
            tigerObject = this.gameObject;

        tigerAnimator = GetComponent<Animator>();
        
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

    public void AppearTiger()
    {
        if (!tigerObject.activeSelf)
        {
            tigerObject.SetActive(true);
            FadeUtility.Instance?.FadeIn(tigerObject, fadeDuration, 0f);
        }
    }
}
