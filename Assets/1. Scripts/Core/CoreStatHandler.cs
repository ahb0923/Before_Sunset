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

    // 기본 스탯
    private int _baseMaxHp = 500;
    private float _baseAttackPower = 10f;
    private float _baseAttackRange = 5f;
    private float _baseSightRange = 8f;

    // 현재 적용된 보너스
    private float _currentHpBonus = 0f;
    private float _currentAttackPowerBonus = 0f;
    private float _currentAttackRangeBonus = 0f;
    private float _currentSightRangeBonus = 0f;

    private void Awake()
    {
        Stats = new CoreUpgradeStats();
        // 게임 시작시 현재 업그레이드 레벨에 맞춰 스탯 적용
        Invoke(nameof(ApplyAllUpgrades), 0.1f); // UpgradeManager 초기화 후에 실행
    }

    /// <summary>
    /// 모든 업그레이드를 다시 적용 (게임 로드시 사용)
    /// </summary>
    private void ApplyAllUpgrades()
    {
        if (UpgradeManager.Instance == null) return;

        ResetToBaseStats();

        // 현재 레벨에 맞춰 모든 업그레이드 적용
        ApplyHPUpgrade(UpgradeManager.Instance.GetCurrentCoreUpgradeEffect(CORE_STATUS_TYPE.HP));
        ApplyAttackPowerUpgrade(UpgradeManager.Instance.GetCurrentCoreUpgradeEffect(CORE_STATUS_TYPE.AttackDamage));
        ApplyAttackRangeUpgrade(UpgradeManager.Instance.GetCurrentCoreUpgradeEffect(CORE_STATUS_TYPE.AttackRange));
        ApplySightRangeUpgrade(UpgradeManager.Instance.GetCurrentCoreUpgradeEffect(CORE_STATUS_TYPE.SightRange));
    }

    private void ResetToBaseStats()
    {
        _currentHpBonus = 0f;
        _currentAttackPowerBonus = 0f;
        _currentAttackRangeBonus = 0f;
        _currentSightRangeBonus = 0f;
    }

    // 개별 업그레이드 적용 메서드들
    public void ApplyHPUpgrade(float increaseAmount)
    {
        _currentHpBonus += increaseAmount;
        UpdateStats();
    }

    public void ApplyAttackPowerUpgrade(float increaseAmount)
    {
        _currentAttackPowerBonus += increaseAmount;
        UpdateStats();
    }

    public void ApplyAttackRangeUpgrade(float increaseAmount)
    {
        _currentAttackRangeBonus += increaseAmount;
        UpdateStats();
    }

    public void ApplySightRangeUpgrade(float increaseAmount)
    {
        _currentSightRangeBonus += increaseAmount;
    }

    private void UpdateStats()
    {
        Stats.MaxHp = Mathf.RoundToInt(_baseMaxHp + _currentHpBonus);
        Stats.AttackPower = _baseAttackPower + _currentAttackPowerBonus;
        Stats.AttackRange = _baseAttackRange + _currentAttackRangeBonus;
    }

    public float GetSightRange()
    {
        return _baseSightRange + _currentSightRangeBonus;
    }
}
