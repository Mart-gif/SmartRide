using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfigDatabase", menuName = "Game/Level Config Database")]
public class LevelConfigDatabase : ScriptableObject
{
    public LevelConfig[] levels;

    public LevelConfig GetLevelConfig(int levelIndex)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] != null && levels[i].levelIndex == levelIndex)
                return levels[i];
        }

        return null;
    }
}