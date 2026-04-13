using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerPathController player;

    [Header("Level Settings")]
    [SerializeField] private LevelConfigDatabase levelConfigDatabase;
    [SerializeField] private LevelContainer[] levelContainers;

    [Header("UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject bottomPanel;

    [SerializeField] private Button winNextButton;
    [SerializeField] private Button winRestartButton;
    [SerializeField] private Button winMenuButton;
    [SerializeField] private Button loseRestartButton;
    [SerializeField] private Button loseMenuButton;

    [SerializeField] private Animator winAnimator;
    [SerializeField] private Animator loseAnimator;
    [SerializeField] private WinPanelUI winPanelUI;
    [SerializeField] private FuelUI fuelCounterUI;

    [Header("Crash FX")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject verticalExplosionPrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip crashSfx;
    [SerializeField] private float loseDelayAfterCrash = 0.5f;

    private int currentLevelIndex = 1;
    private int movesUsed = 0;
    private int currentFuel = 0;
    private bool collectedLevelStar = false;
    private bool levelFinished = false;
    private LevelConfig currentLevelConfig;

    private TaxiNPC[] currentLevelTaxis;
    private BusNPC[] currentLevelBuses;
    private TrafficLightController[] currentLevelTrafficLights;

    private readonly List<CoinCollectible> collectedCoinsThisRun = new List<CoinCollectible>();

    private void Start()
    {
        currentLevelIndex = LevelSession.SelectedLevelIndex;

        if (levelConfigDatabase != null)
            currentLevelConfig = levelConfigDatabase.GetLevelConfig(currentLevelIndex);

        ActivateCurrentLevelContainer();
        CacheCurrentLevelTaxis();
        CacheCurrentLevelBuses();
        CacheCurrentLevelTrafficLights();

        if (currentLevelConfig == null)
        {
            Debug.LogError("Не найден LevelConfig для уровня " + currentLevelIndex);
            return;
        }

        SetupLevelState();
        SpawnPlayerFromLevelContainer();
        InitializeLevelCoins();
        SubscribePlayerEvents();
        SetupUI();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnMoveCompleted -= HandlePlayerMoveCompleted;
            player.OnCrashIntoObstacle -= HandleCrashIntoObstacle;
        }
    }

    private void SetupLevelState()
    {
        movesUsed = 0;
        currentFuel = currentLevelConfig.maxFuel;
        collectedLevelStar = false;
        levelFinished = false;
        collectedCoinsThisRun.Clear();
    }

    private void SetupUI()
    {
        UpdateFuelUI();

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (winNextButton != null)
        {
            winNextButton.onClick.RemoveAllListeners();
            winNextButton.onClick.AddListener(OnNextClicked);
        }

        if (winRestartButton != null)
        {
            winRestartButton.onClick.RemoveAllListeners();
            winRestartButton.onClick.AddListener(OnRestartClicked);
        }

        if (winMenuButton != null)
        {
            winMenuButton.onClick.RemoveAllListeners();
            winMenuButton.onClick.AddListener(OnMenuClicked);
        }

        if (loseRestartButton != null)
        {
            loseRestartButton.onClick.RemoveAllListeners();
            loseRestartButton.onClick.AddListener(OnRestartClicked);
        }

        if (loseMenuButton != null)
        {
            loseMenuButton.onClick.RemoveAllListeners();
            loseMenuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    private void SubscribePlayerEvents()
    {
        if (player == null)
            return;

        player.OnMoveCompleted += HandlePlayerMoveCompleted;
        player.OnCrashIntoObstacle += HandleCrashIntoObstacle;
    }

    private void SpawnPlayerFromLevelContainer()
    {
        if (player == null)
            return;

        LevelContainer currentContainer = GetCurrentLevelContainer();
        if (currentContainer == null)
        {
            Debug.LogError("Не найден LevelContainer для уровня " + currentLevelIndex);
            return;
        }

        if (currentContainer.StartNode == null)
        {
            Debug.LogError("У LevelContainer не назначен StartNode для уровня " + currentLevelIndex);
            return;
        }

        player.SetSpawnNode(currentContainer.StartNode, currentContainer.StartDirection);
    }

    private LevelContainer GetCurrentLevelContainer()
    {
        for (int i = 0; i < levelContainers.Length; i++)
        {
            if (levelContainers[i] != null && levelContainers[i].LevelIndex == currentLevelIndex)
                return levelContainers[i];
        }

        return null;
    }

    private void ActivateCurrentLevelContainer()
    {
        for (int i = 0; i < levelContainers.Length; i++)
        {
            if (levelContainers[i] == null)
                continue;

            bool shouldBeActive = levelContainers[i].LevelIndex == currentLevelIndex;
            levelContainers[i].gameObject.SetActive(shouldBeActive);
        }
    }

    private void CacheCurrentLevelTaxis()
    {
        LevelContainer currentContainer = GetCurrentLevelContainer();

        if (currentContainer == null)
        {
            currentLevelTaxis = new TaxiNPC[0];
            return;
        }

        currentLevelTaxis = currentContainer.GetComponentsInChildren<TaxiNPC>(true);
    }

    private void CacheCurrentLevelBuses()
    {
        LevelContainer currentContainer = GetCurrentLevelContainer();

        if (currentContainer == null)
        {
            currentLevelBuses = new BusNPC[0];
            return;
        }

        currentLevelBuses = currentContainer.GetComponentsInChildren<BusNPC>(true);
    }

    private void CacheCurrentLevelTrafficLights()
    {
        LevelContainer currentContainer = GetCurrentLevelContainer();

        if (currentContainer == null)
        {
            currentLevelTrafficLights = new TrafficLightController[0];
            return;
        }

        currentLevelTrafficLights = currentContainer.GetComponentsInChildren<TrafficLightController>(true);
    }

    private void InitializeLevelCoins()
    {
        CoinCollectible[] coins = FindObjectsOfType<CoinCollectible>(true);

        for (int i = 0; i < coins.Length; i++)
            coins[i].Initialize(this);
    }

    private void HandlePlayerMoveCompleted()
    {
        if (levelFinished)
            return;

        movesUsed++;

        currentFuel--;
        if (currentFuel < 0)
            currentFuel = 0;

        UpdateFuelUI();

        MoveTaxis();
        MoveBuses();
        AdvanceTrafficLights();

        if (currentFuel <= 0)
            LoseLevel();
    }

    private void MoveTaxis()
    {
        if (currentLevelTaxis == null)
            return;

        for (int i = 0; i < currentLevelTaxis.Length; i++)
        {
            if (currentLevelTaxis[i] != null && currentLevelTaxis[i].gameObject.activeInHierarchy)
                currentLevelTaxis[i].StepTaxi();
        }
    }

    private void MoveBuses()
    {
        if (currentLevelBuses == null)
            return;

        for (int i = 0; i < currentLevelBuses.Length; i++)
        {
            if (currentLevelBuses[i] != null && currentLevelBuses[i].gameObject.activeInHierarchy)
                currentLevelBuses[i].StepBus();
        }
    }

    private void AdvanceTrafficLights()
    {
        if (currentLevelTrafficLights == null)
            return;

        for (int i = 0; i < currentLevelTrafficLights.Length; i++)
        {
            if (currentLevelTrafficLights[i] != null && currentLevelTrafficLights[i].gameObject.activeInHierarchy)
                currentLevelTrafficLights[i].AdvanceLight();
        }
    }

    private void HandleCrashIntoObstacle(Vector3 crashPosition, MoveDirection direction)
    {
        TriggerCrash(crashPosition, direction);
    }

    public void TriggerCrash(Vector3 crashPosition, MoveDirection direction)
    {
        if (levelFinished)
            return;

        StartCoroutine(CrashLoseSequence(crashPosition, direction));
    }

    private IEnumerator CrashLoseSequence(Vector3 crashPosition, MoveDirection direction)
    {
        levelFinished = true;

        GameObject explosionToSpawn = explosionPrefab;

        if ((direction == MoveDirection.Up || direction == MoveDirection.Down) && verticalExplosionPrefab != null)
            explosionToSpawn = verticalExplosionPrefab;

        if (explosionToSpawn != null)
            Instantiate(explosionToSpawn, crashPosition, Quaternion.identity);

        if (audioSource != null && crashSfx != null)
            audioSource.PlayOneShot(crashSfx);

        yield return new WaitForSeconds(loseDelayAfterCrash);

        ShowLosePanel();
    }

    private void UpdateFuelUI()
    {
        if (fuelCounterUI != null)
            fuelCounterUI.SetFuel(currentFuel);
    }

    public void AddFuel(int amount)
    {
        if (levelFinished)
            return;

        currentFuel += amount;

        if (currentLevelConfig != null)
            currentFuel = Mathf.Min(currentFuel, currentLevelConfig.maxFuel);

        UpdateFuelUI();
    }

    public void OnLevelStarCollected()
    {
        if (levelFinished)
            return;

        collectedLevelStar = true;
    }

    public void RegisterCollectedCoin(CoinCollectible coin)
    {
        if (levelFinished || coin == null)
            return;

        if (!collectedCoinsThisRun.Contains(coin))
            collectedCoinsThisRun.Add(coin);
    }

    public void CompleteLevel()
    {
        if (levelFinished)
            return;

        levelFinished = true;

        CommitCollectedCoins();

        int stars = CalculateStars();
        LevelProgressManager.SetStars(currentLevelIndex, stars);

        if (topPanel != null)
            topPanel.SetActive(false);

        if (bottomPanel != null)
            bottomPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winPanelUI != null)
            winPanelUI.ShowStars(stars);

        if (winAnimator != null)
            winAnimator.SetTrigger("Show");
    }

    private int CalculateStars()
    {
        int stars = 1;

        if (movesUsed <= currentLevelConfig.movesForTwoStars)
            stars++;

        if (collectedLevelStar)
            stars++;

        return stars;
    }

    private void CommitCollectedCoins()
    {
        int totalCoinsToAdd = 0;

        for (int i = 0; i < collectedCoinsThisRun.Count; i++)
        {
            CoinCollectible coin = collectedCoinsThisRun[i];
            if (coin == null)
                continue;

            if (!CoinsManager.IsCoinCollectedPermanently(coin.CoinUniqueId))
            {
                CoinsManager.MarkCoinAsCollectedPermanently(coin.CoinUniqueId);
                totalCoinsToAdd += coin.CoinValue;
            }
        }

        if (totalCoinsToAdd > 0)
            CoinsManager.AddCoins(totalCoinsToAdd);
    }

    private void LoseLevel()
    {
        if (levelFinished)
            return;

        levelFinished = true;
        ShowLosePanel();
    }

    private void ShowLosePanel()
    {
        if (topPanel != null)
            topPanel.SetActive(false);

        if (bottomPanel != null)
            bottomPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(true);

        if (loseAnimator != null)
            loseAnimator.SetTrigger("Show");
    }

    private void OnNextClicked()
    {
        int nextLevelIndex = currentLevelIndex + 1;
        LevelConfig nextLevelConfig = null;

        if (levelConfigDatabase != null)
            nextLevelConfig = levelConfigDatabase.GetLevelConfig(nextLevelIndex);

        if (nextLevelConfig == null)
        {
            Debug.Log("Следующего уровня пока нет");
            return;
        }

        LevelSession.SelectedLevelIndex = nextLevelIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnRestartClicked()
    {
        LevelSession.SelectedLevelIndex = currentLevelIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMenuClicked()
    {
        SceneManager.LoadScene("Menu");
    }
}