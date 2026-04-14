using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusNPC : MonoBehaviour
{
    private enum BusState
    {
        OnRoad,
        HalfEntering,
        HalfExiting
    }

    [Header("References")]
    [SerializeField] private PlayerPathController player;
    [SerializeField] private GameSceneController gameSceneController;
    [SerializeField] private SpriteRenderer visualSpriteRenderer;

    [Header("Route")]
    [SerializeField] private PathNode startNode;
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Right;
    [SerializeField] private BusTunnelPortal portalA;
    [SerializeField] private BusTunnelPortal portalB;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.15f;
    [SerializeField] private Vector2 worldOffset = Vector2.zero;

    [Header("Sprites")]
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Sprite halfEnterSprite;
    [SerializeField] private Sprite halfExitSprite;

    private PathNode currentNode;
    private BusState state = BusState.OnRoad;
    private BusTunnelPortal enterPortal;
    private BusTunnelPortal exitPortal;
    private bool isMoving;

    private void Awake()
    {
        if (visualSpriteRenderer == null)
            visualSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        currentNode = startNode;
        SnapToCurrentNode();
    }

    public void StepBus()
    {
        if (isMoving || currentNode == null)
            return;

        switch (state)
        {
            case BusState.OnRoad:
                StepOnRoad();
                break;

            case BusState.HalfEntering:
                StepHalfEntering();
                break;

            case BusState.HalfExiting:
                StepHalfExiting();
                break;
        }
    }

    private void StepOnRoad()
    {
        PathNode nextNode = currentNode.GetNeighbor(moveDirection);
        if (nextNode == null)
            return;

        BusTunnelPortal portal = GetPortalByNode(nextNode);

        if (portal != null)
        {
            enterPortal = portal;
            state = BusState.HalfEntering;

            enterPortal.ShowOccupied();
            StartCoroutine(MoveRoutine(nextNode, halfEnterSprite));
            return;
        }

        StartCoroutine(MoveRoutine(nextNode, GetNormalSprite(moveDirection)));
    }

    private void StepHalfEntering()
    {
        if (enterPortal == null)
            return;

        exitPortal = GetOtherPortal(enterPortal);
        if (exitPortal == null || exitPortal.PortalNode == null)
            return;

        enterPortal.ShowNormal();

        moveDirection = exitPortal.ExitDirection;
        state = BusState.HalfExiting;

        exitPortal.ShowOccupied();
        StartCoroutine(MoveRoutine(exitPortal.PortalNode, halfExitSprite));
    }

    private void StepHalfExiting()
    {
        if (exitPortal == null || exitPortal.PortalNode == null)
            return;

        PathNode nextNode = exitPortal.PortalNode.GetNeighbor(moveDirection);
        if (nextNode == null)
            return;

        exitPortal.ShowNormal();
        state = BusState.OnRoad;

        StartCoroutine(MoveRoutine(nextNode, GetNormalSprite(moveDirection)));
    }

    private IEnumerator MoveRoutine(PathNode targetNode, Sprite targetSprite)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = targetNode.transform.position + (Vector3)worldOffset;
        float elapsed = 0f;

        if (visualSpriteRenderer != null && targetSprite != null)
            visualSpriteRenderer.sprite = targetSprite;

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

        CheckCrashWithPlayer();
    }

    private void CheckCrashWithPlayer()
    {
        if (player == null || gameSceneController == null || player.CurrentNode == null)
            return;

        if (player.CurrentNode == currentNode)
        {
            gameSceneController.TriggerCrash(player.transform.position, moveDirection);
        }
    }

    private void SnapToCurrentNode()
    {
        if (currentNode == null)
            return;

        transform.position = currentNode.transform.position + (Vector3)worldOffset;

        if (visualSpriteRenderer != null)
            visualSpriteRenderer.sprite = GetNormalSprite(moveDirection);
    }

    private BusTunnelPortal GetPortalByNode(PathNode node)
    {
        if (portalA != null && portalA.PortalNode == node)
            return portalA;

        if (portalB != null && portalB.PortalNode == node)
            return portalB;

        return null;
    }

    private BusTunnelPortal GetOtherPortal(BusTunnelPortal portal)
    {
        if (portal == portalA) return portalB;
        if (portal == portalB) return portalA;
        return null;
    }

    private Sprite GetNormalSprite(MoveDirection direction)
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
}