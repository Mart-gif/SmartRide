using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelRuntimeData
{
    public int movesUsed;
    public bool collectedLevelStar;

    public void Reset()
    {
        movesUsed = 0;
        collectedLevelStar = false;
    }
}