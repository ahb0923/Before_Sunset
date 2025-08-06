using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DROPITEM_TYPE
{
    Mineral,
    Jewel
}
[System.Serializable]
public class OreDatabase
{
    public int id;
    public string itemName;
    public string prefabName;
    public int dropItemId;
    public DROPITEM_TYPE dropItemType;
    public int hp;
    public int def;
    public int spawnProbability;
    public int spawnStage;

    // 아직 데이터 없음
    public string context;

}
