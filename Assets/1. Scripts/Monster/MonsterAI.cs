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
    private bool _isTargetAlive => Target != null && Target.gameObject.activeInHierarchy;
    private int _targetWalkableId => DefenseManager.Instance.GetWalkableId(Target);
    private NodePath _path;

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
        // 코어가 부서지면, invalid 상태 전환 - 아무것도 안함
        if(DefenseManager.Instance.Core.IsDead)
        {
            ChangeState(MONSTER_STATE.Invalid);
            yield break;
        }

        // 경로와 타겟 모두 존재하고 경로가 이동 가능하면, 이동 상태 전환
        if(_path != null && _isTargetAlive)
        {
            if(_path.IsWalkablePath(_monster.Stat.Size, _targetWalkableId))
            {
                ChangeState(MONSTER_STATE.Move);
                yield break;
            }
        }

        // 코어를 향한 경로가 있다면, 이동 상태 전환
        Vector3 startPos = transform.position;
        Target = DefenseManager.Instance.Core.transform;
        _path = DefenseManager.Instance.FindPathToTarget(startPos, _monster.Stat.Size, Target);
        if(_path != null)
        {
            ChangeState(MONSTER_STATE.Move);
            yield break;
        }

        // 해시셋은 이미 탐색을 진행한 타겟인지를 확인하는 용도
        HashSet<Transform> closedSet = new HashSet<Transform>();
        int count = 0;
        while(_path == null)
        {
            // 인터럽트 발생하면, 코루틴 탈출
            if (IsInterrupted)
                yield break;

            // 이후 타겟은 코어에서 체비쇼프 거리 기준 가까운 타겟 리스트를 순차적으로 가져옴
            List<Transform> targetList = DefenseManager.Instance.GetTargetList(count);
            if(targetList != null && targetList.Count > 0)
            {
                // 타겟 리스트 중에서 현재 몬스터와 가장 가까운 타겟을 탐색
                Target = GetNearestTarget(targetList, closedSet);
                if(Target != null)
                {
                    closedSet.Add(Target);
                    _path = DefenseManager.Instance.FindPathToTarget(startPos, _monster.Stat.Size, Target);
                }
            }

            count++;
            yield return null;
        }

        // 경로를 찾으면, 이동 상태 전환
        ChangeState(MONSTER_STATE.Move);
    }

    /// <summary>
    /// 현재 위치에서 다음 노드의 위치로 이동
    /// </summary>
    private IEnumerator C_Move()
    {
        // 다음 노드가 이동 불가능하면, 탐색 상태 전환
        while (AstarAlgorithm.IsAreaWalkable_Bind(_path.CurNode, _monster.Stat.Size, _targetWalkableId))
        {
            // 인터럽트 발생하면, 코루틴 탈출
            if (IsInterrupted)
                yield break;

            // 일시 정지 시에는 이동 중지
            if (TimeManager.Instance.IsGamePause)
            {
                yield return null;
                continue;
            }

            // 공격 범위 안에 타겟이 감지되면, 공격 상태 전환
            if (_monster.Sensor.DetectedObstacles.Contains(Target))
            {
                ChangeState(MONSTER_STATE.Attack);
                yield break;
            }

            // 실제 오브젝트 이동
            transform.position = Vector2.MoveTowards(transform.position, _path.CurNode.WorldPos, _monster.Stat.Speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _path.CurNode.WorldPos) <= 0.5f)
            {
                //_path.CurNode.hasMonster = false;

                // 마지막 노드 도달하면, 탐색 상태 전환 (아마 무조건 도달 전에 공격 or 죽음 상태로 전환될 듯)
                if (!_path.Next())
                {
                    break;
                }

                //_path.CurNode.hasMonster = true;
            }

            yield return null;
        }

        ChangeState(MONSTER_STATE.Explore);
    }

    /// <summary>
    /// 타겟에 대한 1회 공격
    /// </summary>
    private IEnumerator C_Attack()
    {
        // 타겟이 없어지면, 탐색 상태 전환
        while(_isTargetAlive)
        {
            // 인터럽트 발생하면, 코루틴 탈출
            if (IsInterrupted)
                yield break;

            // 일시 정지 시에는 공격 중지
            if (TimeManager.Instance.IsGamePause)
            {
                yield return null;
                continue;
            }

            switch (_monster.Stat.Type)
            {
                // 원거리 타입은 사거리에 들어오면 원거리 공격
                case MONSTER_TYPE.Ranged:
                    BaseProjectile proj = Instantiate(_monster.Projectile).GetComponent<BaseProjectile>();
                    proj.Init(Target.gameObject, 10, _monster.Stat.AttackPower, transform.position);
                    break;

                // 근접과 탱크 타입은 사거리에 들어오면 근접 공격
                case MONSTER_TYPE.Melee:
                case MONSTER_TYPE.Tank:
                    DamagedSystem.Instance.Send(new Damaged
                    {
                        Attacker = gameObject,
                        Victim = Target.gameObject,
                        Value = _monster.Stat.AttackPower,
                        IgnoreDefense = false
                    });

                    Debug.Log($"{Target}에게 {_monster.Stat.AttackPower}만큼의 데미지!");
                    break;
            }

            // Attack Per Sec 동안 기다림
            yield return _monster.Stat.WaitAttack;
        }

        ChangeState(MONSTER_STATE.Explore);
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
    /// 타겟 리스트 중 오브젝트와 가장 가까운 타겟을 반환
    /// </summary>
    private Transform GetNearestTarget(List<Transform> targetList, HashSet<Transform> closed)
    {
        float dist = float.MaxValue;
        Transform nearest = null;

        foreach (Transform target in targetList)
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
    /// 스폰되었을 때 상태 초기화
    /// </summary>
    public void InitExploreState()
    {
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
