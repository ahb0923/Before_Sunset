using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TOWER_BUILD_TYPE
{
    Base,
    Upgrade
}

[System.Serializable]
public class TowerData
{
    public int id;
    public string towerName;
    public string flavorText;
    public TOWER_BUILD_TYPE buildType;
    public TOWER_ATTACK_TYPE attackType;
    public int nextUpgradeId;

    public int level;
    public float towerHp;
    public float damage;
    public float aps;
    public float range;

    public Dictionary<string, int> buildRequirements;
}

