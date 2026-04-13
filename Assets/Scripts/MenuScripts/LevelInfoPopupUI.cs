using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text starsText;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button closeButton;

    private int currentLevelIndex;
    private LevelSelectManager manager;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        if (replayButton != null)
            replayButton.onClick.AddListener(OnReplayClicked);

        Hide();
    }

    public void Initialize(LevelSelectManager levelSelectManager)
    {
        manager = levelSelectManager;
    }

    public void ShowForLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;

        if (titleText != null)
            titleText.text = "Уровень " + levelIndex;

        if (starsText != null)
            starsText.text = "Звезды: " + LevelProgressManager.GetStars(levelIndex) + "/3";

        if (root != null)
            root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void OnReplayClicked()
    {
        if (manager != null)
            manager.ReplayPassedLevel(currentLevelIndex);
    }
}