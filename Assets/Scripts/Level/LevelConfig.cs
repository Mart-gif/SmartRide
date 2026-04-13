using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Info")]
    public int levelIndex = 1;

    [Header("Player Spawn")]
    public Vector2Int startGridPosition = Vector2Int.zero;
    public MoveDirection startDirection = MoveDirection.Up;

    [Header("Stars")]
    public int movesForTwoStars = 10;

    [Header("Fuel")]
    public int maxFuel = 15;
}