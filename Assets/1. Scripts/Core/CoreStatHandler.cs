using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreUpgradeStats
{
    public int Level = 1;
    public int MaxHp = 500;
    public float AttackPower = 10f;
    public float AttackRange = 5f;
    public float AttackCooldown = 2f;
}

public class CoreStatHandler : MonoBehaviour
{
    [SerializeField] private string coreName;
    public string CoreName => coreName;

    public CoreUpgradeStats Stats { get; private set; } = new CoreUpgradeStats();

    private void Awake()
    {
        Stats = new CoreUpgradeStats();
    }

    public void Upgrade()
    {
        Stats.Level++;
        Stats.MaxHp += 200;
        Stats.AttackPower += 5f;
        Stats.AttackRange += 1f;
        Stats.AttackCooldown = Mathf.Max(0.5f, Stats.AttackCooldown - 0.2f);
    }
}
