using UnityEngine;
using UnityEngine.UIElements;

public class TouchDetector : MonoBehaviour
{
    public GameObject explosionPrefab;
    public PanelPlacement _panelPlacement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Touch detected with " + other.name);
        Debug.Log(other.transform.position);
        if (_panelPlacement != null && _panelPlacement.IsPlacing())
        {
            Debug.Log("Panel is being placed, no explosion instantiated.");
            Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
        }
    }
}
