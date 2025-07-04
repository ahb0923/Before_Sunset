using System.Collections.Generic;

[System.Serializable]
public class SpawnData
{
    public int id;
    public int spawnCount;
}

[System.Serializable]
public class WaveData
{
    public float waitTime;
    public List<SpawnData> spawnDatas;
}

[System.Serializable]
public class StageData
{
    public int stage;
    public List<WaveData> waveDatas;
}
