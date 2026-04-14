using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNodeUI : MonoBehaviour
{
    [SerializeField] private int levelIndex = 1;
    [SerializeField] private Button button;
    [SerializeField] private GameObject completedMark;
    [SerializeField] private GameObject lockedMark;
    [SerializeField] private Image levelImage;
    [SerializeField] private GameObject levelNumber;

    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite starOn;
    [SerializeField] private Sprite starOff;

    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite passedSprite;

    private RectTransform rectTransform;
    private LevelSelectManager manager;

    public int LevelIndex => levelIndex;
    public RectTransform RectTransform => rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (button == null)
            button = GetComponent<Button>();
    }

    public void Initialize(LevelSelectManager levelSelectManager)
    {
        manager = levelSelectManager;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }

        RefreshState();
    }

    public void RefreshState()
    {
        bool isUnlocked = LevelProgressManager.IsLevelUnlocked(levelIndex);
        bool isPassed = LevelProgressManager.IsLevelPassed(levelIndex);

        if (button != null)
            button.interactable = isUnlocked;

        if (completedMark != null)
            completedMark.SetActive(isPassed);

        if (lockedMark != null)
            lockedMark.SetActive(!isUnlocked);

        if (levelNumber != null)
            levelNumber.SetActive(isUnlocked);

        if (levelImage != null)
        {
            if (!isUnlocked)
                levelImage.sprite = lockedSprite;
            else if (isPassed)
                levelImage.sprite = passedSprite;
            else
                levelImage.sprite = unlockedSprite;
        }
        int starCount = Mathf.Clamp(LevelProgressManager.GetStars(levelIndex), 0, stars.Length);

        if (stars != null && stars.Length > 0)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] == null)
                    continue;

                stars[i].sprite = (i < starCount) ? starOn : starOff;
            }
        }
    }

    private void OnClicked()
    {
        if (manager != null)
            manager.OnLevelNodeClicked(this);
    }
}