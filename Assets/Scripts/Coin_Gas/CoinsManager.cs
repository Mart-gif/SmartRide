using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoinsManager
{
    private const string CoinsKey = "TotalCoins";
    private const string CoinCollectedPrefix = "CollectedCoin_";

    public static int GetCoins()
    {
        return PlayerPrefs.GetInt(CoinsKey, 0);
    }

    public static void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        int current = GetCoins();
        current += amount;

        PlayerPrefs.SetInt(CoinsKey, current);
        PlayerPrefs.Save();
    }

    public static bool IsCoinCollectedPermanently(string coinUniqueId)
    {
        if (string.IsNullOrEmpty(coinUniqueId))
            return false;

        return PlayerPrefs.GetInt(CoinCollectedPrefix + coinUniqueId, 0) == 1;
    }

    public static void MarkCoinAsCollectedPermanently(string coinUniqueId)
    {
        if (string.IsNullOrEmpty(coinUniqueId))
            return;

        PlayerPrefs.SetInt(CoinCollectedPrefix + coinUniqueId, 1);
        PlayerPrefs.Save();
    }

    public static void ResetAllCoins()
    {
        PlayerPrefs.DeleteKey(CoinsKey);
        PlayerPrefs.Save();
    }
}