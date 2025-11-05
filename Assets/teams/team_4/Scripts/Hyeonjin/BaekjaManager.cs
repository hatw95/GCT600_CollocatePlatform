using UnityEngine;

public class BaekjaManager : MonoBehaviour
{
    public static BaekjaManager Instance;

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

    public void OnBaekjaCollision(GameObject baekja1, GameObject baekja2)
    {
        BaekjaHandler.Instance.SpawnFusedBaekja(baekja1, baekja2);
    }
}
