using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [Header("Neighbors")]
    [SerializeField] private PathNode up;
    [SerializeField] private PathNode down;
    [SerializeField] private PathNode left;
    [SerializeField] private PathNode right;

    public PathNode GetNeighbor(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Up: return up;
            case MoveDirection.Down: return down;
            case MoveDirection.Left: return left;
            case MoveDirection.Right: return right;
            default: return null;
        }
    }
}
