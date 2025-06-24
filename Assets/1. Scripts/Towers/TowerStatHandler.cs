using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerStatHandler : MonoBehaviour //, IDamagable, IInteractable
{
    [Header(" [ Data ] ")]
    [SerializeField] private Tower_SO _so;

    private BaseTower _tower;

    public event Action<float, float> OnHpChanged;
    public Coroutine fixCoroutine;

    public int Tier { get; private set; }
    public string TowerName { get; private set; }
    public string Context { get; private set; }

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
    public float ProjectileSpeed { get; set; }
    public ResourceRequirement BuildRequirements { get; private set; }
    public ResourceRequirement UpgradeRequirements { get; private set; }

    public SpriteRenderer iconRenderer;
    private bool isFixingDelay = false;

    /// <summary>
    /// 풀링 사용시 OnGet될때 초기화 호출<br/>
    /// 사실 currHp만 다시 세팅해주면 됨 나머지는 괜찮을듯.
    /// </summary>
    /// <param name="damage">원본 데미지 값</param>
    public void Init()
    {
        _tower = GetComponent<BaseTower>();

        if (_so == null)
        {
            Debug.Log("SO데이터 Attach 누락");
            return;
        }
        Tier = _so.tier;
        TowerName = _so.towerName;
        Context = _so.context;
        MaxHp = _so.maxHp;
        // C_Construction 최대 HP 비례해서 회복시킬 예정
        CurrHp = 0;
        AttackPower = _so.attackPower;
        AttackSpeed = _so.attackSpeed;
        AttackRange = _so.attackRange;

        BuildRequirements = _so.buildRequirements;
        UpgradeRequirements = _so.upgradeRequirements;  
    }


    public void UpgradeTower()
    {



    }
}
