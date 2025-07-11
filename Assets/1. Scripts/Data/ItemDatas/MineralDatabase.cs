using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MINERAL_TYPE
{
    Mineral,
    Ingot
}
[System.Serializable]
public class MineralDatabase : ItemDatabase
{
    public MINERAL_TYPE itemType;
    public int ingotId;
}
