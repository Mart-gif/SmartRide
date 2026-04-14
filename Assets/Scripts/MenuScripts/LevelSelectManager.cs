using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private RectTransform carRectTransform;
    [SerializeField] private LevelNodeUI[] levelNodes;
    [SerializeField] private RectTransform mapContent;
    [SerializeField] private LevelInfoPopupUI popupUI;
    [SerializeField] private Button rideButton;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private StarsCounterUI starsCounterUI;
    [Header("Car Sprites")]
    [SerializeField] private Image carImage;
    [SerializeField] private Sprite carRight;
    [SerializeField] private Sprite carLeft;
    [SerializeField] private Sprite carUp;
    [SerializeField] private Sprite carDown;

    [Header("Behavior")]
    [SerializeField] private bool placeCarOnCurrentLevelOnStart = true;

    private bool isBusy;
    private int selectedLevelIndex;

    private void Start()
    {    
        //PlayerPrefs.DeleteAll();
        //для удаления данных прогресса игрока
        InitializeNodes();
        UpdateTotalStars();
        if (popupUI != null)
            popupUI.Initialize(this);


        if (placeCarOnCurrentLevelOnStart)
        {
            selectedLevelIndex = 1;

            for (int i = 1; i <= levelNodes.Length; i++)
            {
                if (LevelProgressManager.IsLevelUnlocked(i))
                    selectedLevelIndex = i;
            }

            PlaceCarOnLevel(selectedLevelIndex);
        }
    }

    private void InitializeNodes()
    {
        for (int i = 0; i < levelNodes.Length; i++)
        {
            if (levelNodes[i] != null)
                levelNodes[i].Initialize(this);
        }
    }

    public void OnLevelNodeClicked(LevelNodeUI node)
    {
        if (isBusy || node == null)
            return;

        int levelIndex = node.LevelIndex;

        if (!LevelProgressManager.IsLevelUnlocked(levelIndex))
            return;

        selectedLevelIndex = levelIndex;
        PlaceCarOnLevel(levelIndex);

        if (popupUI != null)
            popupUI.ShowForLevel(levelIndex);
    }

    public void ReplayPassedLevel(int levelIndex)
    {
        if (isBusy)
            return;

        isBusy = true;

        if (popupUI != null)
            popupUI.Hide();

        PlaceCarOnLevel(levelIndex);
        OpenLevel(levelIndex);
    }

    public void OnRideClicked()
    {
        Debug.Log("Ride clicked. Selected level = " + selectedLevelIndex);
        if (isBusy)
            return;

        LevelNodeUI node = GetNodeByLevel(selectedLevelIndex);

        if (node == null)
        {
            Debug.LogWarning("Не найдена нода для уровня " + selectedLevelIndex);
            return;
        }

        isBusy = true;
        PlaceCarOnLevel(selectedLevelIndex);
        OpenLevel(selectedLevelIndex);
    }

    private void OpenLevel(int levelIndex)
    {
        LevelSession.SelectedLevelIndex = levelIndex;
        SceneManager.LoadScene("SampleScene");
    }

    private void PlaceCarOnLevel(int levelIndex)
    {
        LevelNodeUI node = GetNodeByLevel(levelIndex);

        if (node == null)
            return;

        Vector2 levelPos = node.RectTransform.anchoredPosition;

        if (carRectTransform != null)
        {
            SetCarDirection(levelPos);
            carRectTransform.anchoredPosition = levelPos;
        }

        if (scrollRect != null && mapContent != null)
        {
            RectTransform viewport = scrollRect.viewport;

            float contentWidth = mapContent.rect.width;
            float contentHeight = mapContent.rect.height;

            float viewWidth = viewport.rect.width;
            float viewHeight = viewport.rect.height;

            float normalizedX = (levelPos.x + contentWidth * 0.5f - viewWidth * 0.5f) / (contentWidth - viewWidth);
            float normalizedY = (levelPos.y + contentHeight * 0.5f - viewHeight * 0.5f) / (contentHeight - viewHeight);

            normalizedX = Mathf.Clamp01(normalizedX);
            normalizedY = Mathf.Clamp01(normalizedY);

            scrollRect.normalizedPosition = new Vector2(normalizedX, normalizedY);
        }
        carImage.SetNativeSize();
    }

    private LevelNodeUI GetNodeByLevel(int levelIndex)
    {
        for (int i = 0; i < levelNodes.Length; i++)
        {
            if (levelNodes[i] != null && levelNodes[i].LevelIndex == levelIndex)
                return levelNodes[i];
        }

        return null;
    }   
    private void SetCarDirection(Vector2 targetPos)
    {
        Vector2 currentPos = carRectTransform.anchoredPosition;
        Vector2 dir = targetPos - currentPos;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0)
                carImage.sprite = carRight;
            else
                carImage.sprite = carLeft;
        }
        else
        {
            if (dir.y > 0)
                carImage.sprite = carUp;
            else
                carImage.sprite = carDown;
        }
    }
    private void UpdateTotalStars()
    {
        int total = LevelProgressManager.GetTotalStars(levelNodes.Length);

        if (starsCounterUI != null)
            starsCounterUI.SetNumber(total);
    }
}