using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerData
{
    public int id;
    public int tier;
    public string towerName;
    public string context;
    public float maxHp;
    public float attackPower;
    public float attackSpeed;
    public float attackRange;

    public ResourceCost buildRequirements = new();
    public ResourceCost upgradeRequirements = new();

    public UpgradeStatBonus upgradeStatBonus = new();
}

[System.Serializable]
public class ResourceCost
{
    public Dictionary<int, int> requirements = new();
}

[System.Serializable]
public class UpgradeStatBonus
{
    public float maxHp;
    public float attackPower;
    public float attackSpeed;
    public float attackRange;
}