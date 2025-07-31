using System.Collections.Generic;

[System.Serializable]
public class IntKeyValuePair
{
    public int key;
    public int value;

    public IntKeyValuePair(int key, int value)
    {
        this.key = key;
        this.value = value;
    }

    // IntKeyValuePair로의 암시적 변환
    public static implicit operator IntKeyValuePair(KeyValuePair<int, int> pair)
    {
        return new IntKeyValuePair(pair.Key, pair.Value);
    }
}

[System.Serializable]
public class TwoKeysValuePair
{
    public int key;
    public Portal.PortalDirection direction;
    public int value;

    public TwoKeysValuePair(int key, Portal.PortalDirection direction, int value)
    {
        this.key = key;
        this.direction = direction;
        this.value = value;
    }

    // Int2KeyValuePair로의 암시적 변환
    public static implicit operator TwoKeysValuePair(KeyValuePair<(int, Portal.PortalDirection), int> pair)
    {
        return new TwoKeysValuePair(pair.Key.Item1, pair.Key.Item2, pair.Value);
    }
}

[System.Serializable]
public class MapLinkSaveData
{
    public int currentMapIndex;
    public int nextMapIndex;

    public List<int> mapHistory;
    public List<IntKeyValuePair> prefabIdDict;
    public List<TwoKeysValuePair> portalMapLinks;

    public MapLinkSaveData()
    {
        mapHistory = new List<int>();
        prefabIdDict = new List<IntKeyValuePair>();
        portalMapLinks = new List<TwoKeysValuePair>();
    }
}
