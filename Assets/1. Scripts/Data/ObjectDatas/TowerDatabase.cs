using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TOWER_BUILD_TYPE
{
    Base,
    Upgrade
}

[System.Serializable]
public class TowerDatabase
{
    public int id;
    public string towerName;
    public string flavorText;
    public TOWER_BUILD_TYPE buildType;
    public TOWER_ATTACK_TYPE attackType;
    public int nextUpgradeId;
    public int projectileId;

    public int level;
    public float towerHp;
    public float damage;
    public float aps;
    public float range;

    public string prefabName;

    public Dictionary<string, int> buildRequirements;
}