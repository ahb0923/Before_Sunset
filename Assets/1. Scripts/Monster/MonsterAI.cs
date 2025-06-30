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

    public Transform Target { get; private set; }
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
    /// 코어를 향한 A* 경로 탐색 → 경로 없으면 가장 빠른 경로를 방해하는 타워 탐색
    /// </summary>
    private IEnumerator C_Explore()
    {
        _path = FindPathToCore();
        if( _path != null)
        {
            ChangeState(MONSTER_STATE.Move);
            yield break;
        }
        else
        {
            // 새로운 타겟과 그에 따른 경로를 받아와야 함
        }
    }

    /// <summary>
    /// 코어를 향한 A* 경로 탐색
    /// </summary>
    private NodePath FindPathToCore()
    {
        Node startNode = AstarAlgorithm.GetNodeFromWorldPosition(transform.position);
        return AstarAlgorithm.FindPath(startNode, _core.transform, _core.Size, false); // 대각선 이동 미포함
    }

    /// <summary>
    /// 현재 위치에서 다음 노드의 위치로 이동
    /// </summary>
    private IEnumerator C_Move()
    {
        // 경로에 이동 불가능 노드가 포함되면, 다시 탐색 진행
        if (!_path.IsWalkablePath())
        {
            ChangeState(MONSTER_STATE.Explore);
            yield break;
        }

        while(_path.CurNode.isWalkable)
        {
            if (IsInterrupted)
                yield break;

            transform.position = Vector2.MoveTowards(transform.position, _path.CurNode.WorldPos, _monster.Stat.Speed * Time.deltaTime);

            if(Vector2.Distance(transform.position, _path.CurNode.WorldPos) < 0.01f)
            {
                transform.position = _path.CurNode.WorldPos;

                // 다음 노드가 없다면, 다시 탐색 진행
                if (!_path.Next())
                {
                    ChangeState(MONSTER_STATE.Explore);
                }
                yield break;
            }

            yield return null;
        }

        // 가야하는 노드가 이동 불가능 노드가 되면, 다시 탐색 진행
        ChangeState(MONSTER_STATE.Explore);
    }

    /// <summary>
    /// 타겟에 대한 1회 공격
    /// </summary>
    private IEnumerator C_Attack()
    {
        // 타겟이 없을 경우 탐색 진행
        if(Target == null)
        {
            ChangeState(MONSTER_STATE.Explore);
            yield break;
        }

        // 타겟에 대한 대미지 부여는 여기서 진행
        Debug.Log($"{_monster.Stat.MonsterName} : 공격!");

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
        Target = _core.transform;
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
    /// 해당 오브젝트 선택 시 경로 시각화
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if(_path != null)
        {
            Gizmos.color = Color.red;

            Vector3 pos = _path.Path[0].WorldPos;
            for (int i = 1; i < _path.Path.Count; i++)
            {
                Gizmos.DrawLine(pos, _path.Path[i].WorldPos);
                pos = _path.Path[i].WorldPos;
            }
        }
    }
}
