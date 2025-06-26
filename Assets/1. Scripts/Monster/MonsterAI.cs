using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTER_STATE
{
    Invalid,
    Idle,
    Move,
    Attack,
    Dead,
}

public class MonsterAI : StateBasedAI<MONSTER_STATE>
{
    private MonsterHandler _monster;

    public Transform Target { get; private set; }
    public List<Node> Path { get; private set; }

    protected override MONSTER_STATE InvalidState => MONSTER_STATE.Invalid;

    protected override void OnAwake()
    {
        _monster = GetComponent<MonsterHandler>();
    }

    protected override void DefineStates()
    {
        AddState(MONSTER_STATE.Idle, new StateElem
        {

        });

        AddState(MONSTER_STATE.Move, new StateElem
        {
            Entered = () => Debug.Log("이동 시작"),
            Doing = C_Move,
            Exited = () => Debug.Log("이동 종료")
        });

        AddState(MONSTER_STATE.Attack, new StateElem
        {
            Entered = () => Debug.Log("공격 시작"),
            Doing = C_Attack,
            Exited = () => Debug.Log("공격 종료")
        });

        AddState(MONSTER_STATE.Dead, new StateElem
        {
            Entered = () => PoolManager.Instance.ReturnToPool(POOL_TYPE.Monster, gameObject)
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
        if(Path == null) yield break;

        int index = 0;
        while(index < Path.Count)
        {
            transform.position = Vector2.MoveTowards(transform.position, Path[index].WorldPos, _monster.Stat.Speed * Time.deltaTime);
            if(Vector2.Distance(transform.position, Path[index].WorldPos) < 0.01f)
            {
                transform.position = Path[index].WorldPos;
                index++;
            }
            yield return null;
        }
    }

    private IEnumerator C_Attack()
    {
        yield return null;
    }

    /// <summary>
    /// 타겟과 길을 추가하고 이동 상태로 변환
    /// </summary>
    /// <param name="target"></param>
    /// <param name="path"></param>
    public void AddTarget(Transform target, List<Node> path)
    {
        Target = target;
        Path = path;

        TransitionTo(MONSTER_STATE.Move, true);

        // 상태 변환이 끝난 후에 Doing 코루틴 시작
        RunDoingState();
    }

    public void Dead()
    {
        CurState = MONSTER_STATE.Dead;
    }
}
