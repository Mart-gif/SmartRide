using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusNPC : MonoBehaviour
{
    private enum BusState
    {
        OnRoad,
        HalfEnter,
        HalfExit
    }

    [Header("Refs")]
    [SerializeField] private PlayerPathController player;
    [SerializeField] private GameSceneController gameSceneController;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Path")]
    [SerializeField] private PathNode startNode;
    private PathNode currentNode;
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Right;
    [SerializeField] private BusTunnelPortal portalA;
    [SerializeField] private BusTunnelPortal portalB;

    [Header("Sprites")]
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Sprite halfEnterSprite;
    [SerializeField] private Sprite halfExitSprite;

    [Header("Move")]
    [SerializeField] private float moveDuration = 0.15f;

    private BusState state = BusState.OnRoad;
    private BusTunnelPortal enterPortal;
    private BusTunnelPortal exitPortal;
    private bool isMoving;

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

            case BusState.HalfEnter:
                StepTeleportToExit();
                break;

            case BusState.HalfExit:
                StepExitFromTunnel();
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
            state = BusState.HalfEnter;
            StartCoroutine(MoveRoutine(nextNode, halfEnterSprite));
            return;
        }

        StartCoroutine(MoveRoutine(nextNode, GetNormalSprite(moveDirection)));
    }

    private void StepTeleportToExit()
    {
        exitPortal = GetOtherPortal(enterPortal);
        if (exitPortal == null || exitPortal.PortalNode == null)
            return;

        currentNode = exitPortal.PortalNode;
        transform.position = currentNode.transform.position;

        moveDirection = exitPortal.ExitDirection;

        if (spriteRenderer != null && halfExitSprite != null)
            spriteRenderer.sprite = halfExitSprite;

        state = BusState.HalfExit;

        CheckCrash();
    }

    private void StepExitFromTunnel()
    {
        PathNode nextNode = currentNode.GetNeighbor(moveDirection);
        if (nextNode == null)
            return;

        state = BusState.OnRoad;
        StartCoroutine(MoveRoutine(nextNode, GetNormalSprite(moveDirection)));
    }

    private IEnumerator MoveRoutine(PathNode targetNode, Sprite targetSprite)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = targetNode.transform.position;
        float elapsed = 0f;

        if (spriteRenderer != null && targetSprite != null)
            spriteRenderer.sprite = targetSprite;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        currentNode = targetNode;
        isMoving = false;

        CheckCrash();
    }

    private void CheckCrash()
    {
        if (player == null || gameSceneController == null)
            return;

        if (player.CurrentNode == currentNode)
            gameSceneController.TriggerCrash(player.transform.position, moveDirection);
    }

    private void SnapToCurrentNode()
    {
        if (currentNode == null)
            return;

        transform.position = currentNode.transform.position;

        if (spriteRenderer != null)
            spriteRenderer.sprite = GetNormalSprite(moveDirection);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (player == null || gameSceneController == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        gameSceneController.TriggerCrash(player.transform.position, moveDirection);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (player == null || gameSceneController == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        gameSceneController.TriggerCrash(player.transform.position, moveDirection);
    }
}