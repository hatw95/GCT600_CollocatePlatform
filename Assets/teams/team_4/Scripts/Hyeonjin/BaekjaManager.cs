using UnityEngine;

public class BaekjaManager : MonoBehaviour
{
    public static BaekjaManager Instance;
    [SerializeField] public GameObject perfectBaekja;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnBaekjaCollision(GameObject baekja1, GameObject baekja2, GameObject perfectBaekja)
    {
        BaekjaHandler.Instance.SpawnFusedBaekja(baekja1, baekja2, perfectBaekja);
    }
}
