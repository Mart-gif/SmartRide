using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNodeUI : MonoBehaviour
{
    [Header("Neighbors")]
    [SerializeField] private PathNodeUI up;
    [SerializeField] private PathNodeUI down;
    [SerializeField] private PathNodeUI left;
    [SerializeField] private PathNodeUI right;

    private RectTransform rectTransform;

    public RectTransform RectTransform => rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public PathNodeUI GetNeighbor(MoveDirection direction)
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