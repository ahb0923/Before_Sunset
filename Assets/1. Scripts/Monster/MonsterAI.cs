using System.Collections;
using UnityEngine;

public enum MONSTER_STATE
{
    Invalid,
    Explore,
    Move,
    Attack,
    Dead,
}

public class MonsterAI : StateBasedAI<MONSTER_STATE>
{
    private BaseMonster _monster;
    private Core _core;

    public GameObject Target { get; private set; }
    private NodePath _path;

    protected override MONSTER_STATE InvalidState => MONSTER_STATE.Invalid;

    public void Init(BaseMonster monster, Core core)
    {
        _monster = monster;
        _core = core;
    }

    protected override void DefineStates()
    {
        AddState(MONSTER_STATE.Explore, new StateElem
        {
            Entered = () => Debug.Log("탐색 시작"),
            Doing = C_Explore,
            Exited = () => Debug.Log("탐색 종료")
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

    /// <summary>
    /// 코어를 향한 경로 탐색 → 경로 없으면, 가장 빠른 경로를 방해하는 타워 탐색
    /// </summary>
    private IEnumerator C_Explore()
    {
        // 코어를 향한 경로 탐색
        _path = AstarAlgorithm.FindPathToCore(AstarAlgorithm.GetNodeFromWorldPosition(transform.position), _monster.Stat.Size);

        if(_path != null)
        {
            // 경로가 있으면, 이동 상태 전환
            ChangeState(MONSTER_STATE.Move);
            yield break;
        }
        else
        {
            // 경로가 없으면, 새로운 타겟과 그에 따른 경로를 받아와야 함
            Debug.Log("타겟을 타워로 변경합니다!");
        }
    }

    /// <summary>
    /// 현재 위치에서 다음 노드의 위치로 이동
    /// </summary>
    private IEnumerator C_Move()
    {
        // 해당 경로가 이동 불가능하면, 탐색 상태 전환 (맨 밑 명령어)
        while(_path.IsWalkablePath(_monster.Stat.Size))
        {
            // 이동하려는 노드가 이동 가능하면, 이동
            while (AstarAlgorithm.IsAreaWalkable(_path.CurNode, _monster.Stat.Size))
            {
                // 인터럽트 발생하면, 코루틴 탈출
                if (IsInterrupted)
                    yield break;

                // 실제 오브젝트 이동
                transform.position = Vector2.MoveTowards(transform.position, _path.CurNode.WorldPos, _monster.Stat.Speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, _path.CurNode.WorldPos) < 0.01f)
                {
                    transform.position = _path.CurNode.WorldPos;

                    // 마지막 노드 도달하면, 탐색 상태 전환 (아마 무조건 도달 전에 공격 or 죽음 상태로 전환될 듯)
                    if (!_path.Next())
                    {
                        ChangeState(MONSTER_STATE.Explore);
                    }

                    // 이동하려는 노드 도착 시, 경로의 이동 가능 검사를 위해 코루틴 탈출
                    break;
                }

                yield return null;
            }
        }

        ChangeState(MONSTER_STATE.Explore);
    }

    /// <summary>
    /// 타겟에 대한 1회 공격
    /// </summary>
    private IEnumerator C_Attack()
    {
        // 타겟이 없을 경우, 탐색 상태 전환
        if(Target == null)
        {
            ChangeState(MONSTER_STATE.Explore);
            yield break;
        }

        // 타겟에 대한 대미지 부여
        DamagedSystem.Instance.Send(new Damaged
        {
            Attacker = gameObject,
            Victim = Target,
            Value = _monster.Stat.AttackPower,
            IgnoreDefense = false
        });
        Debug.Log($"{Target}에게 {_monster.Stat.AttackPower}만큼의 데미지!");

        // Attack Per Sec 동안 기다림
        yield return _monster.Stat.WaitAttack;
    }

    /// <summary>
    /// 사망 애니메이션 & 풀링 반환
    /// </summary>
    private void Dead()
    {
        // 사망 애니메이션 처리
        Debug.Log($"{_monster.Stat.MonsterName} 몬스터 사망");

        PoolManager.Instance.ReturnToPool(_monster.Stat.Id, gameObject);
    }

    /// <summary>
    /// 스폰되었을 때 상태 초기화
    /// </summary>
    public void InitExploreState()
    {
        Target = _core.gameObject;
        _path = null;
        TransitionTo(MONSTER_STATE.Explore, true);

        RunDoingState();
    }

    /// <summary>
    /// 상태 변환
    /// </summary>
    public void ChangeState(MONSTER_STATE state)
    {
        CurState = state;
    }

    /// <summary>
    /// 경로 시각화
    /// </summary>
    private void OnDrawGizmos()
    {
        if(_path != null)
        {
            Gizmos.color = Color.yellow;

            Vector3 pos = _path.Path[0].WorldPos;
            for (int i = 1; i < _path.Path.Count; i++)
            {
                Gizmos.DrawLine(pos, _path.Path[i].WorldPos);
                pos = _path.Path[i].WorldPos;
            }
        }
    }
}
