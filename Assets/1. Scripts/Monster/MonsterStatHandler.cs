using UnityEngine;

public class MonsterStatHandler : MonoBehaviour, IDamageable
{
    private BaseMonster _monster;

    [SerializeField] private MonsterData _data;

    public string MonsterName { get; private set; }
    public MONSTER_TYPE Type { get; private set; }

    public int MaxHp { get; private set; }
    public int CurHp { get; private set; }
    
    public int AttackPower { get; private set; }
    public float AttackPerSec { get; private set; }
    public WaitForSeconds WaitAttack { get; private set; }
    public float AttackRange { get; private set; }
    public float Speed { get; private set; }
    public int Size { get; private set; }
    public string Context { get; private set; }

    public void Init(BaseMonster monster, int id)
    {
        _data = DataManager.Instance.MonsterData.GetById(id);

        if (_data == null)
        {
            Debug.Log("[MonsterStatHandler] 몬스터 데이터가 존재하지 않습니다.");
            return;
        }

        _monster = monster;

        MonsterName = _data.monsterName;
        Type = _data.monsterType;
        MaxHp = _data.hp;
        CurHp = _data.hp;
        AttackPower = _data.damage;
        AttackPerSec = _data.aps;
        WaitAttack = new WaitForSeconds(AttackPerSec);
        AttackRange = _data.range;
        Speed = _data.speed;
        Size = _data.size;
        Context = _data.context;
    }

    public void SetFullHp()
    {
        CurHp = MaxHp;
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

        Debug.Log($"[{damaged.Attacker}]로부터 『{damaged.Value}』데미지 - {gameObject.name}");
        CurHp -= DamageCalculator.CalcDamage(damaged.Value, 0f, damaged.IgnoreDefense);
        CurHp = Mathf.Max(CurHp, 0);

        if (CurHp == 0)
        {
            _monster.Ai.ChangeState(MONSTER_STATE.Dead);
        }
    }
}
