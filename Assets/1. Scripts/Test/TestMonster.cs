using System.Collections;
using UnityEngine;

public class TestMonster : StateBasedAI<MONSTER_STATE>, IPoolable
{
    [SerializeField] private int _testID = 1;
    [SerializeField] private int _maxHp = 30;
    [SerializeField] private int _damage = 5;
    [SerializeField] private float _attackSpeed = 1;
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _attackRange = 1;
    [SerializeField] private LayerMask _attackMask;

    private Transform target;
    private int _curHp;

    protected override MONSTER_STATE InvalidState => MONSTER_STATE.Invalid;

    protected override void DefineStates()
    {
        AddState(MONSTER_STATE.Explore, new StateElem
        {

        });

        AddState(MONSTER_STATE.Move, new StateElem
        {
            Doing = C_Move,
        });

        AddState(MONSTER_STATE.Attack, new StateElem
        {
            Doing = C_Attack,
        });

        AddState(MONSTER_STATE.Dead, new StateElem
        {
            Entered = Dead
        });
    }

    protected override bool IsAIEnded()
    {
        return CurState == MONSTER_STATE.Dead;
    }

    protected override bool IsTerminalState(MONSTER_STATE state)
    {
        return false;
    }

    private IEnumerator C_Move()
    {
        target = GetAttackTarget();
        if (target != null)
        {
            CurState = MONSTER_STATE.Attack;
            yield break;
        }

        Vector3 targetPos = MonsterSpawner.Instance.Core.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);

        yield return null;
    }

    private IEnumerator C_Attack()
    {
        if(target == null)
        {
            CurState = MONSTER_STATE.Move;
            yield break;
        }

        // 타겟에 대한 대미지 부여는 여기서 진행
        Debug.Log($"몬스터 {_testID} : 공격!");

        yield return new WaitForSeconds(_attackSpeed);
    }

    private void Dead()
    {
        // 사망 애니메이션 처리
        Debug.Log($"몬스터 {_testID} : 몬스터 사망");

        PoolManager.Instance.ReturnToPool(GetID(), gameObject);
    }

    public int GetID()
    {
        return _testID;
    }

    public void OnGetFromPool()
    {
        _curHp = _maxHp;
        TransitionTo(MONSTER_STATE.Move, true);
    }

    public void OnReturnToPool()
    {
        
    }

    public Transform GetAttackTarget()
    {
        Collider2D[] detected = new Collider2D[10];
        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, _attackRange, detected, _attackMask);

        if (hitCount > 0)
            return detected[0].transform;
        else
            return null;
    }
}
