using System;
using System.Collections;
using UnityEngine;

public class PlayerPathController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.15f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visualSpriteRenderer;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Vector2 worldOffset = Vector2.zero;

    private bool isMoving;
    private PathNode currentNode;
    private MoveDirection currentDirection = MoveDirection.Up;

    public PathNode CurrentNode => currentNode;
    public MoveDirection CurrentDirection => currentDirection;

    public event Action OnMoveCompleted;
    public event Action<Vector3, MoveDirection> OnCrashIntoObstacle;

    private void Awake()
    {
        if (visualSpriteRenderer == null)
            visualSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetSpawnNode(PathNode spawnNode, MoveDirection spawnDirection)
    {
        currentNode = spawnNode;
        currentDirection = spawnDirection;

        ApplyDirectionSprite(currentDirection);
        SnapToNode();
    }

    public void Move(MoveDirection direction)
    {
        if (isMoving || currentNode == null)
            return;

        PathNode nextNode = currentNode.GetNeighbor(direction);

        if (nextNode == null)
        {
            OnCrashIntoObstacle?.Invoke(transform.position, currentDirection);
            return;
        }

        currentDirection = direction;
        ApplyDirectionSprite(currentDirection);

        currentNode = nextNode;
        StartCoroutine(MoveRoutine(nextNode));
    }

    private IEnumerator MoveRoutine(PathNode targetNode)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 target = targetNode.transform.position + (Vector3)worldOffset;

        float t = 0f;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / moveDuration);
            transform.position = Vector3.Lerp(start, target, lerp);
            yield return null;
        }

        transform.position = target;
        isMoving = false;

        OnMoveCompleted?.Invoke();
    }

    private void SnapToNode()
    {
        transform.position = currentNode.transform.position + (Vector3)worldOffset;
    }

    private void ApplyDirectionSprite(MoveDirection dir)
    {
        if (visualSpriteRenderer == null) return;

        switch (dir)
        {
            case MoveDirection.Up: visualSpriteRenderer.sprite = upSprite; break;
            case MoveDirection.Down: visualSpriteRenderer.sprite = downSprite; break;
            case MoveDirection.Left: visualSpriteRenderer.sprite = leftSprite; break;
            case MoveDirection.Right: visualSpriteRenderer.sprite = rightSprite; break;
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}