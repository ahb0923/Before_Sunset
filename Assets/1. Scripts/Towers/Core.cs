using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour, IDamageable, ISaveable
{
    [SerializeField] private int _size;
    public int Size => _size;
    [SerializeField] private int _maxHp = 500;
    public int MaxHp => _maxHp;
    public int CurHp { private set; get; }
    [SerializeField] private Image _hpBar;
    public bool IsDead { get; private set; }

    private SpriteRenderer _spriter;
    private CoreStatHandler _statHandler;
    private CoreUpgradeStats Stats => _statHandler.Stats;

    private void Awake()
    {
        _spriter = GetComponentInChildren<SpriteRenderer>();
        _statHandler = GetComponent<CoreStatHandler>();
        SetFullHp();
    }

    /// <summary>
    /// 최대 체력
    /// </summary>
    public void UpdateMaxHp(int newMaxHp)
    {
        int hpDifference = newMaxHp - _maxHp;
        _maxHp = newMaxHp;

        // 현재 체력도 증가분만큼 증가 (체력 업그레이드시 즉시 회복)
        SetHp(CurHp + hpDifference);

        Debug.Log($"코어 최대 체력 업데이트: {_maxHp}, 현재 체력: {CurHp}");
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
            TimeManager.Instance.PauseGame(true); // 일단 게임 오버 시에 정지
        }
    }

    /// <summary>
    /// 현재 HP 업데이트
    /// </summary>
    private void SetHp(int hp)
    {
        CurHp = Mathf.Clamp(hp, 0, _maxHp);
        _hpBar.fillAmount = (float)CurHp / _maxHp;
    }

    /// <summary>
    /// 코어 체력 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        data.coreCurHp = CurHp;
    }

    /// <summary>
    /// 코어 체력 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        SetHp(data.coreCurHp);
    }
}
