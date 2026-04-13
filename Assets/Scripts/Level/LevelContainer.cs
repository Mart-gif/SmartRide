using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    [SerializeField] private int levelIndex = 1;
    [SerializeField] private PathNode startNode;
    [SerializeField] private MoveDirection startDirection = MoveDirection.Up;

    public int LevelIndex => levelIndex;
    public PathNode StartNode => startNode;
    public MoveDirection StartDirection => startDirection;
}
