using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UPGRADE_TYPE
{
    MoveSpeed,
    MiningSpeed,
    DropRate,
    SightRange,
    HP,
    AttackRange,
    AttackPower
    //HPRegen
}
public enum UPGRADE_CATEGORY
{
    Player,
    Core
}
public class UpgradeDatabase
{
    public int id;
    public string upgradeName;
    public UPGRADE_TYPE statType;
    public UPGRADE_CATEGORY category;
    public int level;
    public float increaseRate;
    public int essenceCost;
}
