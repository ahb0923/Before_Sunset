using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTER_REWARD_TYPE
{
    EssenceShard,
    Mineral,
    Jewel,
    Ingot
}
public class MonsterRewardDatabase
{
    public int id;
    public int monsterid;
    public string idName;
    public MONSTER_REWARD_TYPE rewardType;
    public float dropProbability;
    public int minQuantity;
    public int maxQuantity;
}
