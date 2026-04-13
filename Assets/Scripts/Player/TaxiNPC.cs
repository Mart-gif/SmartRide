using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiNPC : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerPathController player;
    [SerializeField] private GameSceneController gameSceneController;
    [SerializeField] private PathNode startNode;

    [Header("Movement")]
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Right;
    [SerializeField] private float moveDuration = 0.15f;
    [SerializeField] private Vector2 worldOffset = Vector2.zero;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visualSpriteRenderer;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    private PathNode currentNode;
    private bool isMoving;

    public PathNode CurrentNode => currentNode;
    public bool IsMoving => isMoving;

    private void Awake()
    {
        if (visualSpriteRenderer == null)
            visualSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (startNode != null)
        {
            currentNode = startNode;
            transform.position = currentNode.transform.position + (Vector3)worldOffset;
            ApplyDirectionSprite(moveDirection);
        }
    }

    public void StepTaxi()
    {
        if (isMoving || currentNode == null || gameSceneController == null)
            return;

        PathNode nextNode = currentNode.GetNeighbor(moveDirection);

        // если упЄрлись в конец пр€мой Ч разворачиваемс€
        if (nextNode == null)
        {
            moveDirection = GetOppositeDirection(moveDirection);
            ApplyDirectionSprite(moveDirection);
            nextNode = currentNode.GetNeighbor(moveDirection);
        }

        if (nextNode == null)
            return;

        StartCoroutine(MoveRoutine(nextNode));
    }

    private IEnumerator MoveRoutine(PathNode nextNode)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = nextNode.transform.position + (Vector3)worldOffset;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        currentNode = nextNode;
        isMoving = false;

        CheckCrashWithPlayer();
    }

    private void CheckCrashWithPlayer()
    {
        if (player == null || player.CurrentNode == null)
            return;

        if (player.CurrentNode == currentNode)
        {
            gameSceneController.TriggerCrash(player.transform.position, moveDirection);
        }
    }

    private MoveDirection GetOppositeDirection(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Up: return MoveDirection.Down;
            case MoveDirection.Down: return MoveDirection.Up;
            case MoveDirection.Left: return MoveDirection.Right;
            case MoveDirection.Right: return MoveDirection.Left;
            default: return direction;
        }
    }

    private void ApplyDirectionSprite(MoveDirection direction)
    {
        if (visualSpriteRenderer == null)
            return;

        Sprite targetSprite = null;

        switch (direction)
        {
            case MoveDirection.Up: targetSprite = upSprite; break;
            case MoveDirection.Down: targetSprite = downSprite; break;
            case MoveDirection.Left: targetSprite = leftSprite; break;
            case MoveDirection.Right: targetSprite = rightSprite; break;
        }

        if (targetSprite != null)
            visualSpriteRenderer.sprite = targetSprite;
    }
}
