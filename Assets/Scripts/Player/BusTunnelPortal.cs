using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusTunnelPortal : MonoBehaviour
{
    [SerializeField] private PathNode portalNode;
    [SerializeField] private MoveDirection exitDirection = MoveDirection.Right;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer tunnelSpriteRenderer;
    [SerializeField] private Sprite normalTunnelSprite;
    [SerializeField] private Sprite occupiedTunnelSprite;

    public PathNode PortalNode => portalNode;
    public MoveDirection ExitDirection => exitDirection;

    public void ShowNormal()
    {
        if (tunnelSpriteRenderer != null && normalTunnelSprite != null)
            tunnelSpriteRenderer.sprite = normalTunnelSprite;
    }

    public void ShowOccupied()
    {
        if (tunnelSpriteRenderer != null && occupiedTunnelSprite != null)
            tunnelSpriteRenderer.sprite = occupiedTunnelSprite;
    }
}