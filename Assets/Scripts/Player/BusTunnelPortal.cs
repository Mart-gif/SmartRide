using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusTunnelPortal : MonoBehaviour
{
    [SerializeField] private PathNode portalNode;
    [SerializeField] private MoveDirection exitDirection = MoveDirection.Right;

    public PathNode PortalNode => portalNode;
    public MoveDirection ExitDirection => exitDirection;
}