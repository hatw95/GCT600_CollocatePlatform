using UnityEngine;

public class ModelMenu : MonoBehaviour
{
    public ScreenComm screenComm;
    private int buttonIndex;
    private bool sw = false;
    public Transform screen;
    public GameObject[] models = new GameObject[3];

    void Start()
    {
        // Get all child GameObjects and store them in an array
        for (int i = 0; i < transform.childCount; i++)
        {
            models[i].GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void Update()
    {
        buttonIndex = screenComm.GetButtonIndex();
        if (buttonIndex != -1 && !sw)
        {
            Debug.Log("Button index: " + buttonIndex);
            for (int i = 0; i < models.Length; i++)
            {
                models[i].GetComponent<MeshRenderer>().enabled = false;
            }
            models[buttonIndex].GetComponent<MeshRenderer>().enabled = true;
            models[buttonIndex].transform.position = screen.position + new Vector3(0f, 0f, -0.2f);
            sw = true;
        }
        if (buttonIndex == -1 && sw)
        {
            Debug.Log("Button index: " + buttonIndex);
            sw = false;
        }
    }
}
