using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoliceNPC : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerPathController player;
    [SerializeField] private GameSceneController gameSceneController;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool waitingAfterLostSight;

    [Header("Path")]
    [SerializeField] private PathNode startNode;
    [SerializeField] private MoveDirection startDirection = MoveDirection.Up;

    [Header("Sprites")]
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.15f;

    private PathNode currentNode;
    private MoveDirection currentDirection;
    private bool isMoving;
    private bool isChasing;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        currentNode = startNode;
        currentDirection = startDirection;
        SnapToCurrentNode();
    }

    public void StepPolice()
    {
        if (isMoving || currentNode == null || player == null)
            return;

        // 0. Полиция стоит после потери игрока
        if (waitingAfterLostSight)
        {
            // пока игрок вообще где-то виден с этой клетки — не начинаем заново погоню
            if (TryGetVisibleDirectionToPlayer(out _))
                return;

            // когда игрок полностью исчез из всех линий — можно снова реагировать в будущем
            waitingAfterLostSight = false;
            return;
        }

        // 1. Если полиция уже в погоне
        if (isChasing)
        {
            // Игрок всё ещё виден по текущему направлению
            if (CanSeePlayerInDirection(currentDirection))
            {
                PathNode nextNode = currentNode.GetNeighbor(currentDirection);
                if (nextNode != null)
                    StartCoroutine(MoveRoutine(nextNode));

                return;
            }

            // Игрок скрылся, полиция доезжает до поворота и там замирает
            if (IsTurnNode(currentNode, currentDirection))
            {
                isChasing = false;
                waitingAfterLostSight = true;
                return;
            }

            PathNode continueNode = currentNode.GetNeighbor(currentDirection);
            if (continueNode != null)
            {
                StartCoroutine(MoveRoutine(continueNode));
            }
            else
            {
                isChasing = false;
                waitingAfterLostSight = true;
            }

            return;
        }

        // 2. Если полиция свободна — ищем игрока по всем 4 сторонам
        if (TryGetVisibleDirectionToPlayer(out MoveDirection visibleDirection))
        {
            isChasing = true;
            currentDirection = visibleDirection;
            ApplyDirectionSprite(currentDirection);

            PathNode nextNode = currentNode.GetNeighbor(currentDirection);
            if (nextNode != null)
                StartCoroutine(MoveRoutine(nextNode));
        }
    }

    private bool TryGetVisibleDirectionToPlayer(out MoveDirection foundDirection)
    {
        // Важно: проверяем все 4 стороны, чтобы на перекрёстке полиция видела игрока с любой стороны
        if (CanSeePlayerInDirection(MoveDirection.Up))
        {
            foundDirection = MoveDirection.Up;
            return true;
        }

        if (CanSeePlayerInDirection(MoveDirection.Down))
        {
            foundDirection = MoveDirection.Down;
            return true;
        }

        if (CanSeePlayerInDirection(MoveDirection.Left))
        {
            foundDirection = MoveDirection.Left;
            return true;
        }

        if (CanSeePlayerInDirection(MoveDirection.Right))
        {
            foundDirection = MoveDirection.Right;
            return true;
        }

        foundDirection = currentDirection;
        return false;
    }

    private bool CanSeePlayerInDirection(MoveDirection direction)
    {
        PathNode checkNode = currentNode.GetNeighbor(direction);

        while (checkNode != null)
        {
            if (checkNode == player.CurrentNode)
                return true;

            checkNode = checkNode.GetNeighbor(direction);
        }

        return false;
    }

    private bool IsTurnNode(PathNode node, MoveDirection direction)
    {
        if (node == null)
            return false;

        switch (direction)
        {
            case MoveDirection.Left:
            case MoveDirection.Right:
                return node.GetNeighbor(MoveDirection.Up) != null || node.GetNeighbor(MoveDirection.Down) != null;

            case MoveDirection.Up:
            case MoveDirection.Down:
                return node.GetNeighbor(MoveDirection.Left) != null || node.GetNeighbor(MoveDirection.Right) != null;
        }

        return false;
    }

    private IEnumerator MoveRoutine(PathNode targetNode)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = targetNode.transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        currentNode = targetNode;
        isMoving = false;

        CheckCrash();
    }

    private void CheckCrash()
    {
        if (player == null || player.CurrentNode == null || gameSceneController == null)
            return;

        if (player.CurrentNode == currentNode)
            gameSceneController.TriggerCrash(player.transform.position, currentDirection);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (player == null || gameSceneController == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        gameSceneController.TriggerCrash(player.transform.position, currentDirection);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (player == null || gameSceneController == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        gameSceneController.TriggerCrash(player.transform.position, currentDirection);
    }

    private void SnapToCurrentNode()
    {
        if (currentNode == null)
            return;

        transform.position = currentNode.transform.position;
        ApplyDirectionSprite(currentDirection);
    }

    private void ApplyDirectionSprite(MoveDirection direction)
    {
        if (spriteRenderer == null)
            return;

        switch (direction)
        {
            case MoveDirection.Up:
                spriteRenderer.sprite = upSprite;
                break;
            case MoveDirection.Down:
                spriteRenderer.sprite = downSprite;
                break;
            case MoveDirection.Left:
                spriteRenderer.sprite = leftSprite;
                break;
            case MoveDirection.Right:
                spriteRenderer.sprite = rightSprite;
                break;
        }
    }
}
