using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerPathController player;
    [SerializeField] private GameSceneController gameSceneController;
    [SerializeField] private PathNode lightNode;
    [SerializeField] private SpriteRenderer visualSpriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite greenSprite;

    [Header("State")]
    [SerializeField] private bool startsGreen = true;

    private bool isGreen;

    private void Awake()
    {
        if (visualSpriteRenderer == null)
            visualSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        isGreen = startsGreen;
        RefreshVisual();
    }

    public void AdvanceLight()
    {
        isGreen = !isGreen;
        RefreshVisual();

        if (!isGreen && player != null && lightNode != null && gameSceneController != null)
        {
            if (player.CurrentNode == lightNode && !player.IsMoving())
            {
                gameSceneController.TriggerCrash(player.transform.position, player.CurrentDirection);
            }
        }
    }

    private void RefreshVisual()
    {
        if (visualSpriteRenderer == null)
            return;

        visualSpriteRenderer.sprite = isGreen ? greenSprite : redSprite;
    }
}