using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusNPC : MonoBehaviour
{
    private enum BusState
    {
        OnRoad,
        HalfEnteringTunnel,
        HalfExitingTunnel
    }

    [Header("References")]
    [SerializeField] private PlayerPathController player;
    [SerializeField] private GameSceneController gameSceneController;

    [Header("Route")]
    [SerializeField] private PathNode startHeadNode;
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Right;
    [SerializeField] private BusTunnelPortal portalA;
    [SerializeField] private BusTunnelPortal portalB;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.15f;
    [SerializeField] private Vector2 worldOffset = Vector2.zero;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visualSpriteRenderer;

    [Header("Normal Sprites")]
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    [Header("Tunnel Sprites")]
    [SerializeField] private Sprite halfEnterSprite;
    [SerializeField] private Sprite halfExitSprite;

    private PathNode headNode;
    private BusState state = BusState.OnRoad;
    private BusTunnelPortal targetPortal;
    private BusTunnelPortal exitPortal;
    private bool isMoving;

    private void Start()
    {
        headNode = startHeadNode;
        SnapToNode();
    }

    public void StepBus()
    {
        if (isMoving || headNode == null)
            return;

        switch (state)
        {
            case BusState.OnRoad:
                StepOnRoad();
                break;

            case BusState.HalfEnteringTunnel:
                StepHalfEntering();
                break;

            case BusState.HalfExitingTunnel:
                StepHalfExiting();
                break;
        }
    }

    private void StepOnRoad()
    {
        PathNode nextNode = headNode.GetNeighbor(moveDirection);
        if (nextNode == null)
            return;

        BusTunnelPortal portal = GetPortalByNode(nextNode);

        if (portal != null)
        {
            targetPortal = portal;
            state = BusState.HalfEnteringTunnel;

            // автобус реально доезжает до входного туннеля
            targetPortal.ShowOccupied();
            StartCoroutine(MoveRoutine(nextNode, halfEnterSprite, true));
            return;
        }

        StartCoroutine(MoveRoutine(nextNode, GetNormalSprite(moveDirection), true));
    }

    private void StepHalfEntering()
    {
        if (targetPortal == null)
            return;

        exitPortal = GetOtherPortal(targetPortal);
        if (exitPortal == null || exitPortal.PortalNode == null)
            return;

        // входной туннель возвращаем в обычный вид
        targetPortal.ShowNormal();

        // теперь автобус появляется у второго туннеля наполовину
        state = BusState.HalfExitingTunnel;
        moveDirection = exitPortal.ExitDirection;
        exitPortal.ShowOccupied();

        StartCoroutine(MoveRoutine(exitPortal.PortalNode, halfExitSprite, false));
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

        StartCoroutine(MoveRoutine(nextNode, GetNormalSprite(moveDirection), true));
    }

    private IEnumerator MoveRoutine(PathNode targetNode, Sprite sprite, bool updateHeadNodeAtEnd)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = targetNode.transform.position + (Vector3)worldOffset;
        float elapsed = 0f;

        if (sprite != null && visualSpriteRenderer != null)
            visualSpriteRenderer.sprite = sprite;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;

        if (updateHeadNodeAtEnd)
            headNode = targetNode;
        else
            headNode = targetNode;

        isMoving = false;

        CheckCrash();
    }

    private void CheckCrash()
    {
        if (player == null || player.CurrentNode == null || gameSceneController == null)
            return;

        if (player.CurrentNode == headNode || player.CurrentNode == GetTailNode())
        {
            gameSceneController.TriggerCrash(player.transform.position, moveDirection);
        }
    }

    private PathNode GetTailNode()
    {
        if (headNode == null)
            return null;

        return headNode.GetNeighbor(GetOppositeDirection(moveDirection));
    }

    private void SnapToNode()
    {
        if (headNode == null)
            return;

        transform.position = headNode.transform.position + (Vector3)worldOffset;

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