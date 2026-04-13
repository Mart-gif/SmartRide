using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private string coinUniqueId;
    [SerializeField] private int coinValue = 1;

    [Header("Optional SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSfx;

    private bool collectedThisRun = false;
    private GameSceneController gameSceneController;

    public string CoinUniqueId => coinUniqueId;
    public int CoinValue => coinValue;

    public void Initialize(GameSceneController controller)
    {
        gameSceneController = controller;

        if (CoinsManager.IsCoinCollectedPermanently(coinUniqueId))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collectedThisRun)
            return;

        if (!other.CompareTag("Player"))
            return;

        collectedThisRun = true;

        if (audioSource != null && collectSfx != null)
            audioSource.PlayOneShot(collectSfx);

        if (gameSceneController != null)
        {
            gameSceneController.RegisterCollectedCoin(this);
        }

        gameObject.SetActive(false);
    }
}