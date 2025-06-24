using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum TOWER_ATTACK_TYPE
{
    Projectile,         // 투사체
    AoE,                // 자기 중심 공격
    Laser               // 연결 타워
}

[CreateAssetMenu(fileName = "Tower_", menuName = "Scriptable Objects/Tower")]
public class Tower_SO : ScriptableObject
{
    [Header("[ Components ]")]
    public int tier;
    public string towerName;
    public string context;
    public float maxHp;
    public float attackPower;
    public float attackSpeed;
    public float attackRange;

    [Header(" Projectile ")]
    public GameObject projectile;
    //public float projectileSpeed;
    //public float projectileRange;

    [Header("[ Images ]")]
    public Sprite baseImage;
    public Sprite upgradeImage;

    [Header("[ Requirements ]")]
    public ResourceRequirement buildRequirements;

    [Header("[ Upgrade Requirements ]")]
    public ResourceRequirement upgradeRequirements;
}

[System.Serializable]
public struct ResourceRequirement
{
    public int stone;
    public int cooper;
    public int iron;
    public int silver;
    public int gold;

    public int Tanzanite;
    public int Jade;
    public int Opal;
    public int Amethyst;
    public int Sapphire;
    public int Serendibite;
}
