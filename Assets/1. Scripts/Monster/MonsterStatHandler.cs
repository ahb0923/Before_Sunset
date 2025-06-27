using UnityEngine;

public class MonsterStatHandler : MonoBehaviour, IDamageable
{
    private BaseMonster _monster;

    [SerializeField] private MonsterData data;

    public int Id { get; private set; }
    public string MonsterName { get; private set; }
    public MONSTER_TYPE Type { get; private set; }

    public int MaxHp { get; private set; }
    private int _curHp;
    public int CurHp
    {
        get => _curHp;
        set
        {
            _curHp = Mathf.Clamp(value, 0, MaxHp);
            // 0 아래로 떨어지면 사망 처리
        }
    }
    
    public int AttackPower { get; private set; }
    public float AttackPerSec { get; private set; }
    public WaitForSeconds WaitAttack { get; private set; }
    public float AttackRange { get; private set; }
    
    public float Speed { get; private set; }

    public void Init(BaseMonster monster)
    {
        if (data == null)
        {
            Debug.Log("[MonsterStatHandler] 몬스터 데이터가 존재하지 않습니다.");
            return;
        }

        _monster = monster;

        Id = data.id;
        MonsterName = data.monsterName;
        Type = data.type;
        MaxHp = data.hp;
        CurHp = data.hp;
        AttackPower = data.damage;
        AttackPerSec = data.aps;
        WaitAttack = new WaitForSeconds(AttackPerSec);
        AttackRange = data.range;
        Speed = data.speed;
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

        CurHp -= DamageCalculator.CalcDamage(damaged.Value, 0f, damaged.IgnoreDefense);
        CurHp = Mathf.Max(CurHp, 0);

        if (CurHp <= 0)
        {
            CurHp = 0;
            _monster.Ai.ChangeState(MONSTER_STATE.Dead);
        }
    }
}
