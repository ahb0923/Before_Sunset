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

    public Dictionary<string, int> buildRequirements;
    public Dictionary<string, int> upgradeRequirements;

    public UpgradeStatBonus upgradeStatBonus = new();
}

[System.Serializable]
public class UpgradeStatBonus
{
    public float maxHp;
    public float attackPower;
    public float attackSpeed;
    public float attackRange;
}