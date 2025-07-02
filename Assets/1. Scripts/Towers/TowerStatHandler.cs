 using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerStatHandler : MonoBehaviour, IDamageable
{
    [Header(" [ Data ] ")]
    [SerializeField] private TowerData _data;
    [SerializeField] private int _towerID;

    private BaseTower _tower;

    public event Action<float, float> OnHpChanged;
    public Coroutine fixCoroutine;

    public int Level { get; private set; }
    public string TowerName { get; private set; }

    public TOWER_ATTACK_TYPE attackType { get; private set; }

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
    //public ResourceRequirement BuildRequirements { get; private set; }

    public SpriteRenderer iconRenderer;
    //private bool isFixingDelay = false;



    /// <summary>
    /// 풀링 사용시 OnGet될때 초기화 호출<br/>
    /// 사실 currHp만 다시 세팅해주면 됨 나머지는 괜찮을듯.
    /// </summary>
    /// <param name="damage">원본 데미지 값</param>
    public void Init()
    {
        _tower = GetComponent<BaseTower>();
        _data = DataManager.Instance.TowerData.GetById(_towerID);

        if (_data == null)
        {
            Debug.Log("데이터 세팅 누락");
            return;
        }
        Level = _data.level;
        TowerName = _data.towerName;
        FlavorText = _data.flavorText;
        MaxHp = _data.towerHp;
        // C_Construction 최대 HP 비례해서 회복시킬 예정 => 기획변동 없을시 초기화에서 최대값으로
        CurrHp = MaxHp;
        AttackPower = _data.damage;
        AttackSpeed = _data.aps;
        AttackRange = _data.range;
    }

    /// <summary>
    /// <<IDamageable>>
    /// 실제 hp 변동 메서드
    /// </summary>
    /// <param name="damaged">받은 데미지 정보</param>
    public void OnDamaged(Damaged damaged)
    {
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
            _tower.ai.SetState(TOWER_STATE.Destroy, force: true);
        }
    }

    
    /// <summary>
    /// 데미지 테스팅 메서드
    /// </summary>
    [ContextMenu("테스트 데미지 30")]
    private void TestDamage()
    {
        var dummy = new Damaged
        {
            Value = 30,
            IgnoreDefense = false,
            Attacker = this.gameObject
        };

        OnDamaged(dummy);
        Debug.Log($"CurrHp : {CurrHp}");
    }
}
