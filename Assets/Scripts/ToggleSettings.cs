using UnityEngine;

public class ToggleSettings : MonoBehaviour
{
    public GameObject settingsMenu;
    public MeshRenderer meshRenderer;

    public void ToggleSettingsMenu()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
        }
    }

    public void ScreenVisualizeToggle()
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = !meshRenderer.enabled;
        }
    }    
}
