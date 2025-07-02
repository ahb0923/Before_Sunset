using System.Collections;
using System.Collections.Generic;
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

    public Transform Target { get; private set; }
    private NodePath _path;
    private Node _nextNode;

    protected override MONSTER_STATE InvalidState => MONSTER_STATE.Invalid;

    public void Init(BaseMonster monster)
    {
        _monster = monster;
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
        int count = 0;
        HashSet<Transform> closedSet = new HashSet<Transform>();

        while(_path == null)
        {
            Vector3 startPos = _nextNode == null ? transform.position : _nextNode.WorldPos;

            if(count == 0)
            {
                Target = MapManager.Instance.Core.transform;
                count++;
            }
            else
            {
                List<Transform> targetList = MapManager.Instance.GetTargetList(count);
                if(targetList == null || targetList.Count == 0)
                {
                    count++;
                    continue;
                }

                Target = GetNearestTarget(targetList, closedSet);

                if(Target == null)
                {
                    count++;
                    continue;
                }

                closedSet.Add(Target);
            }

            _path = MapManager.Instance.FindPathToTarget(startPos, _monster.Stat.Size, Target);
            yield return null;
        }

        // 경로를 찾으면, 이동 상태 전환
        ChangeState(MONSTER_STATE.Move);
    }

    /// <summary>
    /// 타겟 리스트 중 오브젝트와 가장 가까운 타겟을 반환
    /// </summary>
    private Transform GetNearestTarget(List<Transform> targetList, HashSet<Transform> closed)
    {
        float dist = float.MaxValue;
        Transform nearest = null;

        foreach(Transform target in targetList)
        {
            if (closed.Contains(target)) continue;

            float tempDist = Vector2.Distance(transform.position, target.position);

            if (dist > tempDist)
            {
                dist = tempDist;
                nearest = target;
            }
        }

        return nearest;
    }

    /// <summary>
    /// 현재 위치에서 다음 노드의 위치로 이동
    /// </summary>
    private IEnumerator C_Move()
    {
        _nextNode = _path.CurNode;

        // 해당 경로가 이동 불가능하면, 탐색 상태 전환
        while (_path.IsWalkablePath(_monster.Stat.Size, MapManager.Instance.GetWalkableId(Target)))
        {
            // 인터럽트 발생하면, 코루틴 탈출
            if (IsInterrupted)
                yield break;

            // 실제 오브젝트 이동
            transform.position = Vector2.MoveTowards(transform.position, _nextNode.WorldPos, _monster.Stat.Speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _nextNode.WorldPos) < 0.01f)
            {
                transform.position = _nextNode.WorldPos;

                // 마지막 노드 도달하면, 탐색 상태 전환 (아마 무조건 도달 전에 공격 or 죽음 상태로 전환될 듯)
                if (!_path.Next())
                {
                    break;
                }

                _nextNode = _path.CurNode;
            }

            yield return null;
        }

        _path = null;
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
            Victim = Target.gameObject,
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

        PoolManager.Instance.ReturnToPool(_monster.GetId(), gameObject);
    }

    /// <summary>
    /// 스폰되었을 때 상태 초기화
    /// </summary>
    public void InitExploreState()
    {
        _path = null;
        _nextNode = null;
        ChangeState(MONSTER_STATE.Explore);

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
