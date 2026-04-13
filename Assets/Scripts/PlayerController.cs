using System.Collections;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Grid")]
    [SerializeField] private GridSettings gridSettings;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.15f;

    [Header("Obstacle Check")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Vector2 overlapBoxSize = new Vector2(0.8f, 0.8f);

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visualSpriteRenderer;
    [SerializeField] private Vector2 worldOffset = Vector2.zero;

    [Header("Direction Sprites")]
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    private bool isMoving = false;
    private MoveDirection currentDirection = MoveDirection.Up;
    private Vector2Int currentGridPosition;

    public Vector2Int CurrentGridPosition => currentGridPosition;

    public event Action OnMoveCompleted;

    public event Action<Vector3> OnCrashIntoObstacle;

    private void Awake()
    {
        if (visualSpriteRenderer == null)
            visualSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetSpawnData(Vector2Int spawnGridPosition, MoveDirection spawnDirection)
    {
        currentGridPosition = spawnGridPosition;
        currentDirection = spawnDirection;

        ApplyDirectionSprite(currentDirection);
        SnapToCurrentGridPosition();
    }

    public void Move(MoveDirection direction)
    {
        if (isMoving || gridSettings == null)
            return;

        Vector2Int moveOffset = GetGridOffset(direction);
        Vector2Int targetGridPosition = currentGridPosition + moveOffset;

        if (IsBlocked(targetGridPosition))
        {
            Vector3 crashWorldPosition = GetWorldPositionFromGrid(currentGridPosition);
            OnCrashIntoObstacle?.Invoke(crashWorldPosition);
            return;
        }

        currentDirection = direction;
        ApplyDirectionSprite(currentDirection);

        currentGridPosition = targetGridPosition;

        Vector3 targetWorldPosition = GetWorldPositionFromGrid(currentGridPosition);
        StartCoroutine(MoveRoutine(targetWorldPosition));
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    private bool IsBlocked(Vector2Int targetGridPosition)
    {
        Vector3 targetWorldPosition = gridSettings.GridToWorld(targetGridPosition);

        Collider2D hit = Physics2D.OverlapBox(
            targetWorldPosition,
            overlapBoxSize,
            0f,
            obstacleLayer
        );

        return hit != null;
    }

    private Vector2Int GetGridOffset(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Up: return Vector2Int.up;
            case MoveDirection.Down: return Vector2Int.down;
            case MoveDirection.Left: return Vector2Int.left;
            case MoveDirection.Right: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    private void ApplyDirectionSprite(MoveDirection direction)
    {
        if (visualSpriteRenderer == null)
            return;

        Sprite targetSprite = GetDirectionSprite(direction);

        if (targetSprite != null)
            visualSpriteRenderer.sprite = targetSprite;
    }

    private Sprite GetDirectionSprite(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Up: return upSprite;
            case MoveDirection.Down: return downSprite;
            case MoveDirection.Left: return leftSprite;
            case MoveDirection.Right: return rightSprite;
            default: return null;
        }
    }

    private Vector3 GetWorldPositionFromGrid(Vector2Int gridPosition)
    {
        return gridSettings.GridToWorld(gridPosition) + (Vector3)worldOffset;
    }

    private void SnapToCurrentGridPosition()
    {
        transform.position = GetWorldPositionFromGrid(currentGridPosition);
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;

        OnMoveCompleted?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (gridSettings == null)
            return;

        Gizmos.color = Color.red;

        DrawTargetCell(Vector2Int.up);
        DrawTargetCell(Vector2Int.down);
        DrawTargetCell(Vector2Int.left);
        DrawTargetCell(Vector2Int.right);
    }

    private void DrawTargetCell(Vector2Int offset)
    {
        Vector2Int targetGrid = currentGridPosition + offset;
        Vector3 worldPos = gridSettings.GridToWorld(targetGrid);
        Gizmos.DrawWireCube(worldPos, overlapBoxSize);
    }
}