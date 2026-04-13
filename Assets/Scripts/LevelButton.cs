using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Button button;
    [SerializeField] private bool isUnlocked = true;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    private void Start()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(LoadLevel);
            button.interactable = isUnlocked;
        }
    }

    public void LoadLevel()
    {
        if (!isUnlocked)
            return;

        SceneManager.LoadScene(sceneName);
    }
}