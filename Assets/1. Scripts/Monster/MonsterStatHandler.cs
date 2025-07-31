using System.Collections;
using UnityEngine;

public class MonsterStatHandler : MonoBehaviour, IDamageable
{
    private BaseMonster _monster;

    [SerializeField] private MonsterDatabase _data;
    
    [SerializeField] private Material _hitMat;
    private Material _originMat;
    [SerializeField] private float _hitDuration = 0.1f;
    private Coroutine _hitCoroutine;

    public string MonsterName { get; private set; }
    public ATTACK_TYPE AttackType { get; private set; }
    public MOVE_TYPE MoveType { get; private set; }

    public int MaxHp { get; private set; }
    public int CurHp { get; private set; }

    public int AttackPower { get; private set; }
    public float AttackPerSec { get; private set; }
    public float AttackRange { get; private set; }
    public float Speed { get; set; }
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
        AttackType = _data.attackType;
        MoveType = _data.moveType;
        MaxHp = _data.hp;
        CurHp = _data.hp;
        AttackPower = _data.damage;
        AttackPerSec = _data.aps;
        AttackRange = _data.range;
        Speed = _data.speed;
        Size = _data.size;
        Context = _data.context;

        _originMat = _monster.Spriter.material;
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
        if (_monster.IsDead) return;

        _monster.OnBeforeDamaged?.Invoke(damaged);

        if (damaged.Attacker == null)
        {
            //Debug.LogWarning("타격 대상 못찾음!");
            return;
        }

        //Debug.Log($"[{damaged.Attacker}]로부터 『{damaged.FinalDamage}』데미지 - {gameObject.name}");
        CurHp -= DamageCalculator.CalcDamage(damaged.Value, 0f, damaged.IgnoreDefense, damaged.Multiplier);
        CurHp = Mathf.Max(CurHp, 0);
        _monster.HpBar.UpdateHpBar(CurHp);

        if (CurHp == 0)
        {
            _monster.Ai.ChangeState(MONSTER_STATE.Dead);
        }
        else
        {
            if(_hitCoroutine != null)
            {
                StopCoroutine( _hitCoroutine);
            }
            _hitCoroutine = StartCoroutine(C_Hit());
        }
    }

    private IEnumerator C_Hit()
    {
        _monster.Spriter.material = _hitMat;
        yield return Helper_Coroutine.WaitSeconds(_hitDuration);
        _monster.Spriter.material = _originMat;
        _hitCoroutine = null;
    }
}
