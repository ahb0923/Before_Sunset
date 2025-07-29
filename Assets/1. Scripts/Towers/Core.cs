using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour, IDamageable, ISaveable
{
    [SerializeField] private int _size;
    public int Size => _size;
    [SerializeField] private int _maxHp = 500;
    public int CurHp { private set; get; }
    [SerializeField] private Image _hpBar;
    public bool IsDead { get; private set; }

    private SpriteRenderer _spriter;

    private CoreStatHandler _statHandler;
    private CoreUpgradeStats Stats => _statHandler.Stats;

    private int[] _upgradeCostPerLevel;

    private void Awake()
    {
        _spriter = GetComponentInChildren<SpriteRenderer>();
        _statHandler = GetComponent<CoreStatHandler>();

        SetFullHp();
    }

    /// <summary>
    /// 스테이지 클리어 후 1일차에 코어 체력 모두 회복
    /// </summary>
    public void SetFullHp()
    {
        SetHp(_maxHp);
    }

    /// <summary>
    /// 실제 hp 변동 메서드
    /// </summary>
    /// <param name="damaged">받은 데미지 정보</param>
    public void OnDamaged(Damaged damaged)
    {
        if (IsDead) return;

        if (damaged.Attacker == null)
        {
            Debug.LogWarning("타격 대상 못찾음!");
            return;
        }

        SetHp(Mathf.Max(CurHp - DamageCalculator.CalcDamage(damaged.Value, 0f, damaged.IgnoreDefense), 0));

        if (CurHp == 0)
        {
            IsDead = true;
            _spriter.color = _spriter.color.WithAlpha(0.5f);
            TimeManager.Instance.TestGameOver();
        }
    }

    /// <summary>
    /// 현재 HP 업데이트
    /// </summary>
    private void SetHp(int hp)
    {
        CurHp = hp;
        _hpBar.fillAmount = (float)CurHp / _maxHp;
    }

    /// <summary>
    /// 업그레이드
    /// </summary>
    public bool TryUpgrade()
    {
        int nextLevel = Stats.Level + 1;
        //if (nextLevel >= _upgradeCostPerLevel.Length) return false;

        //int cost = _upgradeCostPerLevel[nextLevel];
        // 정수 차감  return false;

        _statHandler.Upgrade();

        SetHp(Stats.MaxHp);
        return true;
    }


    /// <summary>
    /// 코어 체력 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        data.coreCurHp = CurHp;
        // data.coreLevel = _stats.Level; 데이터에 추가해야됨
    }

    /// <summary>
    /// 코어 체력 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        //_stats.Level = data.coreLevel;
        //// 레벨에 따른 스탯 갱신
        //for (int i = 1; i < data.coreLevel; i++)
        //{
        //    _stats.MaxHp += 200;
        //    _stats.AttackPower += 5f;
        //    _stats.AttackRange += 1f;
        //    _stats.AttackCooldown = Mathf.Max(0.5f, _stats.AttackCooldown - 0.2f);
        //}

        SetHp(data.coreCurHp);
    }
}
