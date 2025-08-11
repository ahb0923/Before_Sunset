 using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerStatHandler : MonoBehaviour, IDamageable
{
    private BaseTower _tower;

    private TowerDatabase _data;

    public event Action<float, float> OnHpChanged;
    //public Coroutine fixCoroutine;

    public int ID { get; private set; }
    public int? ProjectileID { get; private set; }
    public int? DebuffID { get; private set; }
    public int? NextupgradeID { get; private set; }
    public int Level { get; private set; }
    public string TowerName { get; private set; }

    public TOWER_ATTACK_TYPE AttackType { get; private set; }
    public TOWER_BUILD_TYPE BuildType { get; private set; }

    public string FlavorText { get; private set; }

    private float _currHp;
    public float CurrHp
    {
        get => _currHp;
        set
        {
            _currHp = Mathf.Clamp(value, 0, MaxHp);
            OnHpChanged?.Invoke(_currHp, MaxHp);
        }
    }

    public float MaxHp { get; private set; }
    public float AttackPower { get; set; }
    public float AttackSpeed { get; set; }
    public float AttackRange { get; set; }

    // 투사체 속도 => projectile로 이동
    public float ProjectileSpeed { get; set; }
    public Dictionary<string, int> BuildRequirements { get; private set; }
    public Dictionary<string, int> AccumulatedCosts { get; set; }

    //private bool isFixingDelay = false;

    /// <summary>
    /// 풀링 사용시 OnGet될때 초기화 호출<br/>
    /// 사실 currHp만 다시 세팅해주면 됨 나머지는 괜찮을듯.
    /// </summary>
    /// <param name="damage">원본 데미지 값</param>
    public void Init(BaseTower baseTower, int towerId)
    {
        _tower = baseTower;
        _data = DataManager.Instance.TowerData.GetById(towerId);

        if (_data == null)
        {
            Debug.Log("데이터 세팅 누락");
            return;
        }
        ID = _data.id;
        ProjectileID = _data.projectileId;
        DebuffID = _data.debuffId;
        NextupgradeID = _data.nextUpgradeId;
        Level = _data.level;
        TowerName = _data.towerName;
        AttackType = _data.attackType;
        BuildType = _data.buildType;
        FlavorText = _data.flavorText;
        MaxHp = _data.towerHp;
        // C_Construction 최대 HP 비례해서 회복시킬 예정 => 기획변동 없을시 초기화에서 최대값으로
        CurrHp = MaxHp;
        AttackPower = _data.damage;
        AttackSpeed = _data.aps;
        AttackRange = _data.range;
        BuildRequirements = _data.buildRequirements;
        AccumulatedCosts = new Dictionary<string, int>(BuildRequirements);

        _tower._attackCollider.radius = AttackRange + 0.5f;
        _tower.ui.SetEffectSize(AttackRange);
    }

    /// <summary>
    /// <<IDamageable>>
    /// 실제 hp 변동 메서드
    /// </summary>
    /// <param name="damaged">받은 데미지 정보</param>
    public void OnDamaged(Damaged damaged)
    {
        if (_tower.ai.CurState == TOWER_STATE.Destroy) return;

        if (damaged.Attacker == null)
        {
            Debug.LogWarning("타격 대상 못찾음!");
            return;
        }

        CurrHp -= DamageCalculator.CalcDamage(damaged.Value, 0f, damaged.IgnoreDefense);
        CurrHp = Mathf.Max(CurrHp, 0);

        if (CurrHp <= 0)
        {
            CurrHp = 0;
            if (_tower.ai.CurState != TOWER_STATE.Destroy)
                _tower.ai.SetState(TOWER_STATE.Destroy, force: true);
        }
    }
    public void OnFixed(float amount)
    {
        if (_tower.ai.CurState == TOWER_STATE.Destroy) return;
        CurrHp = Mathf.Min(CurrHp + amount, MaxHp);
    }

    /// <summary>
    /// 타워 업그레이드 로직
    /// </summary>
    public void UpgradeTowerStat()
    {
        if (NextupgradeID == null)
            return;
        var upgradeData = DataManager.Instance.TowerData.GetById((int)NextupgradeID);
        bool checkRequirements = true;

        var buildRequirements = upgradeData.buildRequirements;
        if (!GameManager.Instance.GOD_MODE)
        {
            foreach (var item in buildRequirements)
            {
                if (InventoryManager.Instance.Inventory.GetItemCount(item.Key) < item.Value)
                {
                    ToastManager.Instance.ShowToast("자원이 부족합니다!");
                    checkRequirements = false;
                }
            }
        }

        if (!QuestManager.Instance.IsPossibleToAction(QUEST_TYPE.UpgradeTower)) return;

        if (checkRequirements)
        {
            foreach (var item in upgradeData.buildRequirements)
                InventoryManager.Instance.Inventory.UseItem(item.Key, item.Value);
            QuestManager.Instance.AddQuestAmount(QUEST_TYPE.UpgradeTower, NextupgradeID ?? -1);

            ID = upgradeData.id;
            Level = upgradeData.level; 
            MaxHp += upgradeData.towerHp;
            CurrHp += upgradeData.towerHp;
            AttackPower += upgradeData.damage;
            AttackSpeed += upgradeData.aps;
            AttackRange += upgradeData.range;

            _tower._attackCollider.radius = AttackRange + 0.5f;
            _tower.ui.SetEffectSize(AttackRange);

            BuildType = upgradeData.buildType;
            NextupgradeID = upgradeData.nextUpgradeId;

            if (DebuffID != null)
            {
                DebuffID = upgradeData.debuffId;
            }
            if (ProjectileID != null)
            {
                ProjectileID = upgradeData.projectileId;
            }

            foreach (var kvp in upgradeData.buildRequirements)
            {
                if (AccumulatedCosts.ContainsKey(kvp.Key))
                    AccumulatedCosts[kvp.Key] += kvp.Value;
                else
                    AccumulatedCosts[kvp.Key] = kvp.Value;
            }

        }
    }



    /// <summary>
    /// 데미지 테스팅 메서드
    /// </summary>
    [ContextMenu("테스트 데미지 15")]
    private void TestDamage()
    {
        var dummy = new Damaged
        {
            Value = 15,
            IgnoreDefense = false,
            Attacker = this.gameObject
        };

        OnDamaged(dummy);
        Debug.Log($"CurrHp : {CurrHp}");
    }
}
