using System;
using System.Collections.Generic;

public enum PLAYER_STATUS_TYPE
{
    MoveSpeed,
    MiningSpeed,
    DropRate,
    SightRange,
}

public enum CORE_STATUS_TYPE
{
    HP,
    AttackRange,
    AttackDamage,
    SightRange,
}

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    public Dictionary<PLAYER_STATUS_TYPE, int> PlayerStatus { get; private set; }
    public Dictionary<CORE_STATUS_TYPE, int> CoreStatus { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        InitStatus();
    }

    private void InitStatus()
    {
        PlayerStatus = new Dictionary<PLAYER_STATUS_TYPE, int>();
        int ps = Enum.GetValues(typeof(PLAYER_STATUS_TYPE)).Length;
        for (int i = 0; i < ps; i++)
        {
            PlayerStatus.Add((PLAYER_STATUS_TYPE)i, 0);
        }

        CoreStatus = new Dictionary<CORE_STATUS_TYPE, int>();
        int cs = Enum.GetValues(typeof(CORE_STATUS_TYPE)).Length;
        for (int i = 0; i < cs; i++)
        {
            CoreStatus.Add((CORE_STATUS_TYPE)i, 0);
        }
    }
    
    
}
