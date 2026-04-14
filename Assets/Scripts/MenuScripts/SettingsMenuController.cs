using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }


public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }
}