using UnityEngine;

public class PlayerStatHandler : MonoBehaviour
{
    private BasePlayer _player;

    [SerializeField] private int _initialPickaxeId = 700;
    public EquipmentDatabase Pickaxe => (EquipmentDatabase)InventoryManager.Instance.Inventory.Pickaxe.Data;

    [SerializeField] private float _baseMoveSpeed = 2.0f;
    [SerializeField] private float _baseMiningSpeed = 1.0f;
    [SerializeField] private float _baseDropRate = 0.0f;
    [SerializeField] private float _baseSightRange = 5.0f;

    // 현재 적용된 스탯들 (업그레이드 반영)
    private float _currentMoveSpeedMultiplier = 1.0f;
    private float _currentMiningSpeedMultiplier = 1.0f;
    private float _currentDropRateBonus = 0.0f;
    private float _currentSightRangeMultiplier = 1.0f;

    // 최종 스탯 프로퍼티들
    public float MoveSpeed => _baseMoveSpeed * _currentMoveSpeedMultiplier;
    public float MiningSpeed => _baseMiningSpeed * _currentMiningSpeedMultiplier;
    public float DropRate => _baseDropRate + _currentDropRateBonus;
    public float SightRange => _baseSightRange * _currentSightRangeMultiplier;

    public void Init(BasePlayer player)
    {
        _player = player;
        if (InventoryManager.Instance.Inventory.Pickaxe == null)
        {
            InventoryManager.Instance.Inventory.SetPickaxe(DataManager.Instance.EquipmentData.GetById(_initialPickaxeId));
        }

        // 게임 시작시 현재 업그레이드 레벨에 맞춰 스탯 적용
        ApplyAllUpgrades();
    }

    /// <summary>
    /// 모든 업그레이드를 다시 적용 (게임 로드시 사용)
    /// </summary>
    private void ApplyAllUpgrades()
    {
        ResetToBaseStats();

        // 현재 레벨에 맞춰 모든 업그레이드 적용
        ApplyMoveSpeedUpgrade(UpgradeManager.Instance.GetCurrentPlayerUpgradeEffect(PLAYER_STATUS_TYPE.MoveSpeed));
        ApplyMiningSpeedUpgrade(UpgradeManager.Instance.GetCurrentPlayerUpgradeEffect(PLAYER_STATUS_TYPE.MiningSpeed));
        ApplyDropRateUpgrade(UpgradeManager.Instance.GetCurrentPlayerUpgradeEffect(PLAYER_STATUS_TYPE.DropRate));
        ApplySightRangeUpgrade(UpgradeManager.Instance.GetCurrentPlayerUpgradeEffect(PLAYER_STATUS_TYPE.SightRange));
    }

    private void ResetToBaseStats()
    {
        _currentMoveSpeedMultiplier = 1.0f;
        _currentMiningSpeedMultiplier = 1.0f;
        _currentDropRateBonus = 0.0f;
        _currentSightRangeMultiplier = 1.0f;
    }

    // 업그레이드 적용 메서드들
    public void ApplyMoveSpeedUpgrade(float increaseRate)
    {
        _currentMoveSpeedMultiplier = 1.0f + increaseRate;
        Debug.Log($"이동속도 업그레이드 적용: {MoveSpeed}");
    }

    public void ApplyMiningSpeedUpgrade(float increaseRate)
    {
        _currentMiningSpeedMultiplier = 1.0f + increaseRate;
        Debug.Log($"채굴속도 업그레이드 적용: {MiningSpeed}");
    }

    public void ApplyDropRateUpgrade(float increaseRate)
    {
        _currentDropRateBonus += increaseRate;
        Debug.Log($"드랍률 업그레이드 적용: {DropRate}%");
    }

    public void ApplySightRangeUpgrade(float increaseRate)
    {
        _currentSightRangeMultiplier = 1.0f + increaseRate;
        Debug.Log($"시야범위 업그레이드 적용: {SightRange}");
    }
}
