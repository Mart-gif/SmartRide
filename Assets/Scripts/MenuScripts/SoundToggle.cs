using UnityEngine;

public class SoundToggle : MonoBehaviour
{
    [SerializeField] private GameObject mutedIcon;

    private bool isMuted = false;

    public void ToggleSound()
    {
        isMuted = !isMuted;

        AudioListener.volume = isMuted ? 0f : 1f;

        mutedIcon.SetActive(isMuted);
    }
}