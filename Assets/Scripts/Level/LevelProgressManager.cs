using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelProgressManager
{
    private const string UnlockedLevelKey = "UnlockedLevel";
    private const string StarsKeyPrefix = "LevelStars_";

    public static int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(UnlockedLevelKey, 1);
    }

    public static void SetUnlockedLevel(int level)
    {
        int current = GetUnlockedLevel();

        if (level > current)
        {
            PlayerPrefs.SetInt(UnlockedLevelKey, level);
            PlayerPrefs.Save();
        }
    }

    public static bool IsLevelPassed(int levelIndex)
    {
        return GetStars(levelIndex) > 0;
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex == 1)
            return true;

        return IsLevelPassed(levelIndex - 1);
    }

    public static void SetStars(int levelIndex, int stars)
    {
        stars = Mathf.Clamp(stars, 0, 3);

        int currentStars = GetStars(levelIndex);

        if (stars > currentStars)
        {
            PlayerPrefs.SetInt(StarsKeyPrefix + levelIndex, stars);
            PlayerPrefs.Save();
        }

    }

    public static int GetStars(int levelIndex)
    {
        return PlayerPrefs.GetInt(StarsKeyPrefix + levelIndex, 0);
    }

    public static void ResetProgressOnly()
    {
        PlayerPrefs.DeleteKey(UnlockedLevelKey);
        PlayerPrefs.Save();
    }
}