using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public Dictionary<string, int> BaseUpgrade { get; private set; }
    public Dictionary<string, int> FixedUpgrade { get; private set; }
    public Dictionary<string, int> VirtualUpgrade { get; private set; }
    
    public int Essence { get; private set; }
    public int EssencePiece { get; private set; }
    public int VirtualEssence { get; private set; }

    public int UsedEssence { get; private set; }
    public int VirtualUsedEssence { get; private set; }
    public int ResetCounter { get; private set; }
    public int ResetCost => 1 + ResetCounter;

    private UpgradeDataHandler _upgradeDataHandler;
    private PlayerStatHandler _playerStatHandler;
    private CoreStatHandler _coreStatHandler;

    protected override void Awake()
    {
        base.Awake();
        InitStatus();

        _upgradeDataHandler = DataManager.Instance.UpgradeData;
        _playerStatHandler = FindObjectOfType<PlayerStatHandler>();
        _coreStatHandler = FindObjectOfType<CoreStatHandler>();
        
        InitUpgrade();
        BaseUpgrade = new Dictionary<string, int>(FixedUpgrade);
    }

    public void InitUpgrade()
    {
        if (FixedUpgrade != null)
            FixedUpgrade = null;
        
        List<UpgradeDatabase> upgradeList = DataManager.Instance.UpgradeData.GetAllItems();
        List<UpgradeDatabase> upgrades = upgradeList.Where(u => u.level == 0).ToList();
        
        FixedUpgrade = new Dictionary<string, int>();
        for (int i = 0; i < upgrades.Count; i++)
        {
            FixedUpgrade.Add(upgrades[i].upgradeName, upgrades[i].id);
        }
    }

    /// <summary>
    /// 플레이어 스탯 업그레이드 시도
    /// </summary>
    public bool TryUpgradePlayer(PLAYER_STATUS_TYPE statusType)
    {
        int currentLevel = PlayerStatus[statusType];
        int nextLevel = currentLevel + 1;

        // JSON에서 업그레이드 데이터 가져오기
        UpgradeDatabase upgradeData = GetPlayerUpgradeData(statusType, nextLevel);
        if (upgradeData == null)
        {
            Debug.LogWarning($"플레이어 업그레이드 불가능: {statusType} 레벨 {nextLevel}");
            return false;
        }

        // 에센스 비용 확인
        if (!HasEnoughEssence(upgradeData.essenceCost))
        {
            Debug.LogWarning($"에센스 부족: 필요 {upgradeData.essenceCost}");
            return false;
        }

        // 에센스 차감
        ConsumeEssence(upgradeData.essenceCost);

        // 레벨 업데이트
        PlayerStatus[statusType] = nextLevel;

        // 실제 스탯 적용
        ApplyPlayerUpgrade(statusType, upgradeData);

        Debug.Log($"플레이어 {statusType} 업그레이드 완료! 레벨: {nextLevel}");
        return true;
    }

    /// <summary>
    /// 코어 스탯 업그레이드 시도
    /// </summary>
    public bool TryUpgradeCore(CORE_STATUS_TYPE statusType)
    {
        int currentLevel = CoreStatus[statusType];
        int nextLevel = currentLevel + 1;

        // JSON에서 업그레이드 데이터 가져오기
        UpgradeDatabase upgradeData = GetCoreUpgradeData(statusType, nextLevel);
        if (upgradeData == null)
        {
            Debug.LogWarning($"코어 업그레이드 불가능: {statusType} 레벨 {nextLevel}");
            return false;
        }

        // 에센스 비용 확인
        if (!HasEnoughEssence(upgradeData.essenceCost))
        {
            Debug.LogWarning($"에센스 부족: 필요 {upgradeData.essenceCost}");
            return false;
        }

        // 에센스 차감
        ConsumeEssence(upgradeData.essenceCost);

        // 레벨 업데이트
        CoreStatus[statusType] = nextLevel;

        // 실제 스탯 적용
        ApplyCoreUpgrade(statusType, upgradeData);

        Debug.Log($"코어 {statusType} 업그레이드 완료! 레벨: {nextLevel}");
        return true;
    }

    /// <summary>
    /// 플레이어 업그레이드 데이터 가져오기
    /// </summary>
    private UpgradeDatabase GetPlayerUpgradeData(PLAYER_STATUS_TYPE statusType, int level)
    {
        UPGRADE_TYPE upgradeType = ConvertToUpgradeType(statusType);
        return _upgradeDataHandler.GetAllItems()
            .FirstOrDefault(data => data.statType == upgradeType &&
                          data.level == level &&
                          data.category == UPGRADE_CATEGORY.Player);
    }

    /// <summary>
    /// 코어 업그레이드 데이터 가져오기
    /// </summary>
    private UpgradeDatabase GetCoreUpgradeData(CORE_STATUS_TYPE statusType, int level)
    {
        UPGRADE_TYPE upgradeType = ConvertToUpgradeType(statusType);
        return _upgradeDataHandler.GetAllItems()
            .FirstOrDefault(data => data.statType == upgradeType &&
                          data.level == level &&
                          data.category == UPGRADE_CATEGORY.Core);
    }

    /// <summary>
    /// PLAYER_STATUS_TYPE을 UPGRADE_TYPE으로 변환
    /// </summary>
    private UPGRADE_TYPE ConvertToUpgradeType(PLAYER_STATUS_TYPE statusType)
    {
        return statusType switch
        {
            PLAYER_STATUS_TYPE.MoveSpeed => UPGRADE_TYPE.MoveSpeed,
            PLAYER_STATUS_TYPE.MiningSpeed => UPGRADE_TYPE.MiningSpeed,
            PLAYER_STATUS_TYPE.DropRate => UPGRADE_TYPE.DropRate,
            PLAYER_STATUS_TYPE.SightRange => UPGRADE_TYPE.SightRange,
            _ => throw new ArgumentException($"Unknown player status type: {statusType}")
        };
    }

    /// <summary>
    /// CORE_STATUS_TYPE을 UPGRADE_TYPE으로 변환
    /// </summary>
    private UPGRADE_TYPE ConvertToUpgradeType(CORE_STATUS_TYPE statusType)
    {
        return statusType switch
        {
            CORE_STATUS_TYPE.HP => UPGRADE_TYPE.HP,
            CORE_STATUS_TYPE.AttackRange => UPGRADE_TYPE.AttackRange,
            CORE_STATUS_TYPE.AttackDamage => UPGRADE_TYPE.AttackPower,
            CORE_STATUS_TYPE.SightRange => UPGRADE_TYPE.SightRange,
            _ => throw new ArgumentException($"Unknown core status type: {statusType}")
        };
    }

    /// <summary>
    /// 플레이어 업그레이드 적용
    /// </summary>
    private void ApplyPlayerUpgrade(PLAYER_STATUS_TYPE statusType, UpgradeDatabase upgradeData)
    {
        if (_playerStatHandler == null) return;

        switch (statusType)
        {
            case PLAYER_STATUS_TYPE.MoveSpeed:
                _playerStatHandler.ApplyMoveSpeedUpgrade(upgradeData.increaseRate);
                break;
            case PLAYER_STATUS_TYPE.MiningSpeed:
                _playerStatHandler.ApplyMiningSpeedUpgrade(upgradeData.increaseRate);
                break;
            case PLAYER_STATUS_TYPE.DropRate:
                _playerStatHandler.ApplyDropRateUpgrade(upgradeData.increaseRate);
                break;
            case PLAYER_STATUS_TYPE.SightRange:
                _playerStatHandler.ApplySightRangeUpgrade(upgradeData.increaseRate);
                break;
        }
    }

    /// <summary>
    /// 코어 업그레이드 적용
    /// </summary>
    private void ApplyCoreUpgrade(CORE_STATUS_TYPE statusType, UpgradeDatabase upgradeData)
    {
        if (_coreStatHandler == null) return;

        switch (statusType)
        {
            case CORE_STATUS_TYPE.HP:
                _coreStatHandler.ApplyHPUpgrade(upgradeData.increaseRate);
                break;
            case CORE_STATUS_TYPE.AttackRange:
                _coreStatHandler.ApplyAttackRangeUpgrade(upgradeData.increaseRate);
                break;
            case CORE_STATUS_TYPE.AttackDamage:
                _coreStatHandler.ApplyAttackPowerUpgrade(upgradeData.increaseRate);
                break;
            case CORE_STATUS_TYPE.SightRange:
                _coreStatHandler.ApplySightRangeUpgrade(upgradeData.increaseRate);
                break;
        }
    }

    /// <summary>
    /// 다음 플레이어 업그레이드 정보 가져오기
    /// </summary>
    public UpgradeDatabase GetNextPlayerUpgradeInfo(PLAYER_STATUS_TYPE statusType)
    {
        int nextLevel = PlayerStatus[statusType] + 1;
        return GetPlayerUpgradeData(statusType, nextLevel);
    }

    /// <summary>
    /// 다음 코어 업그레이드 정보 가져오기
    /// </summary>
    public UpgradeDatabase GetNextCoreUpgradeInfo(CORE_STATUS_TYPE statusType)
    {
        int nextLevel = CoreStatus[statusType] + 1;
        return GetCoreUpgradeData(statusType, nextLevel);
    }

    /// <summary>
    /// 현재 플레이어 업그레이드 효과 가져오기
    /// </summary>
    public float GetCurrentPlayerUpgradeEffect(PLAYER_STATUS_TYPE statusType)
    {
        int currentLevel = PlayerStatus[statusType];
        if (currentLevel == 0) return 0f;

        UpgradeDatabase currentUpgrade = GetPlayerUpgradeData(statusType, currentLevel);
        return currentUpgrade?.increaseRate ?? 0f;
    }

    /// <summary>
    /// 현재 코어 업그레이드 효과 가져오기
    /// </summary>
    public float GetCurrentCoreUpgradeEffect(CORE_STATUS_TYPE statusType)
    {
        int currentLevel = CoreStatus[statusType];
        if (currentLevel == 0) return 0f;

        UpgradeDatabase currentUpgrade = GetCoreUpgradeData(statusType, currentLevel);
        return currentUpgrade?.increaseRate ?? 0f;
    }

    // 에센스 관련 메서드들 (실제 구현에서는 별도 매니저로 분리)
    private bool HasEnoughEssence(int cost)
    {
        // TODO: 실제 에센스 매니저와 연동
        return true; // 임시로 항상 true 반환
    }

    private void ConsumeEssence(int cost)
    {
        // TODO: 실제 에센스 매니저와 연동
        Debug.Log($"에센스 {cost} 소모");
    }

    public void SetVirtualUpgrade()
    {
        VirtualUpgrade = new Dictionary<string, int>(FixedUpgrade);
    }

    public void DiscardVirtualUpgrade()
    {
        VirtualUpgrade = null;
    }

    public void ChangeVirtualUpgrade(string upgradeName, int id)
    {
        VirtualUpgrade[upgradeName] = id;
    }

    public void FixUpgrade()
    {
        FixedUpgrade = VirtualUpgrade;
    }
    
    public void AddEssencePiece(int amount)
    {
        EssencePiece += amount;

        if (EssencePiece >= 30)
        {
            Essence += EssencePiece / 30;
            EssencePiece %= 30;
        }
        
        UIManager.Instance.EssenceUI.Refresh();
    }

    public void SetVirtualEssence()
    {
        VirtualEssence = Essence;
        VirtualUsedEssence = UsedEssence;
    }

    public void UseVirtualEssence(int amount)
    {
        VirtualEssence -= amount;
        VirtualUsedEssence += amount;
    }

    public void FixEssence()
    {
        Essence = VirtualEssence;
        UsedEssence = VirtualUsedEssence;
    }

    public void FixResetCounter()
    {
        Essence = VirtualEssence + UsedEssence - ResetCost;
        ResetCounter++;
        UsedEssence = 0;
    }
}
