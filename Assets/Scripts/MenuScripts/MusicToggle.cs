using UnityEngine;

public class MusicToggle : MonoBehaviour
{
    [SerializeField] private GameObject mutedIcon;
    [SerializeField] private AudioSource musicSource;

    private bool isMuted = false;

    public void ToggleMusic()
    {
        isMuted = !isMuted;

        musicSource.mute = isMuted;

        mutedIcon.SetActive(isMuted);
    }
}