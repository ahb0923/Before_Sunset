using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUpgradePair
{
    public PLAYER_STATUS_TYPE key;
    public int value;

    public PlayerUpgradePair(PLAYER_STATUS_TYPE key, int value)
    {
        this.key = key;
        this.value = value;
    }

    // 암시적 변환
    public static implicit operator PlayerUpgradePair(KeyValuePair<PLAYER_STATUS_TYPE, int> pair)
    {
        return new PlayerUpgradePair(pair.Key, pair.Value);
    }
}

[System.Serializable]
public class CoreUpgradePair
{
    public CORE_STATUS_TYPE key;
    public int value;

    public CoreUpgradePair(CORE_STATUS_TYPE key, int value)
    {
        this.key = key;
        this.value = value;
    }

    // 암시적 변환
    public static implicit operator CoreUpgradePair(KeyValuePair<CORE_STATUS_TYPE, int> pair)
    {
        return new CoreUpgradePair(pair.Key, pair.Value);
    }
}

[System.Serializable]
public class StringIntPair
{
    public string key;
    public int value;

    public StringIntPair(string key, int value)
    {
        this.key = key;
        this.value = value;
    }

    // 암시적 변환
    public static implicit operator StringIntPair(KeyValuePair<string, int> pair)
    {
        return new StringIntPair(pair.Key, pair.Value);
    }
}

[System.Serializable]
public class EssenceSaveData
{
    public int essence;
    public int essencePiece;
    public int usedEssence;
    public int resetCounter;
    public List<PlayerUpgradePair> playerUpgradeDict;
    public List<CoreUpgradePair> coreUpgradeDict;
    public List<StringIntPair> doneUpgradeDict;

    public EssenceSaveData()
    {
        playerUpgradeDict = new List<PlayerUpgradePair>();
        coreUpgradeDict = new List<CoreUpgradePair>();
        doneUpgradeDict = new List<StringIntPair>();
    }
}
