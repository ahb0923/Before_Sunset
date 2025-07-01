using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour, IDamageable
{
    [SerializeField] private int _size;
    public int Size => _size;
    [SerializeField] private int _maxHp = 500;
    private int _curHp;
    [SerializeField] private Image _hpBar;

    private void Awake()
    {
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
        if (damaged.Attacker == null)
        {
            Debug.LogWarning("타격 대상 못찾음!");
            return;
        }

        SetHp(Mathf.Max(_curHp - DamageCalculator.CalcDamage(damaged.Value, 0f, damaged.IgnoreDefense), 0));

        if (_curHp <= 0)
        {
            _curHp = 0;
            Debug.Log("게임 오버!");
        }
    }

    /// <summary>
    /// 현재 HP 업데이트
    /// </summary>
    private void SetHp(int hp)
    {
        _curHp = hp;
        _hpBar.fillAmount = (float)_curHp / _maxHp;
    }
}
