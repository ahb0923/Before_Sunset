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

    public Dictionary<int, int> buildRequirements;
    public Dictionary<int, int> upgradeRequirements;
}
