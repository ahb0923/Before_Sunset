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

    private void Awake()
    {
        _spriter = GetComponentInChildren<SpriteRenderer>();

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
            TimeManager.Instance.PauseGame(true); // 일단 게임 오버 시에 정지
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
