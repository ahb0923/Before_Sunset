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
    private Animator _animator;

    public Transform Target { get; private set; }
    private bool _isTargetAlive => Target != null && Target.gameObject.activeInHierarchy;
    private NodePath _path;
    private Vector2 _moveDir;
    private WaitForFixedUpdate _waitFixedDeltaTime = new WaitForFixedUpdate();

    [SerializeField] private float _deadAnimDuration = 0.5f;

    protected override MONSTER_STATE InvalidState => MONSTER_STATE.Invalid;

    public void Init(BaseMonster monster, Animator animator)
    {
        _monster = monster;
        _animator = animator;
    }

    protected override void DefineStates()
    {
        AddState(MONSTER_STATE.Explore, new StateElem
        {
            //Entered = () => Debug.Log("탐색 시작"),
            Doing = C_Explore,
            //Exited = () => Debug.Log("탐색 종료")
        });

        AddState(MONSTER_STATE.Move, new StateElem
        {
            Entered = () => 
            {
                _animator?.SetBool(BaseMonster.MOVE, true);
                //Debug.Log("이동 시작");
            },
            Doing = C_Move,
            Exited = () =>
            {
                _animator?.SetBool(BaseMonster.MOVE, false);
                //Debug.Log("이동 종료");
            },
        });

        AddState(MONSTER_STATE.Attack, new StateElem
        {
           // Entered = () => Debug.Log("공격 시작"),
            Doing = C_Attack,
            //Exited = () => Debug.Log("공격 종료")
        });

        AddState(MONSTER_STATE.Dead, new StateElem
        {
            Entered = () => StartCoroutine(C_Dead()),
            Exited = () => _monster.Spriter.color = _monster.Spriter.color.WithAlpha(1f)
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
    /// 타겟과 경로에 대한 탐색 진행
    /// </summary>
    private IEnumerator C_Explore()
    {
        // 코어가 부서지면, invalid 상태 전환 - 아무것도 안함
        if(DefenseManager.Instance.Core.IsDead)
        {
            ChangeState(InvalidState);
            yield break;
        }

        // 1. 탐색 범위 내에 타겟 리스트가 존재하면, 가장 가까운 타겟부터 순차적으로 경로 탐색
        Vector3 startPos = transform.position;
        if(_path != null)
        {
            // 새로운 경로를 받아올 예정이므로, 현재 경로 노드에서 해당 몬스터 제외
            _path.ReleaseMonsterCount();

            // 다음 노드가 이동 가능하면 시작 지점을 다음 노드로 설정
            _path.Next();
            if(_path.CurNode.IsWalkable(-1)) startPos = _path.CurNode.WorldPos;
        }

        HashSet<Transform> closedSet = new HashSet<Transform>();
        if (_monster.Detector.DetectedObstacles.Count > 0)
        {
            do
            {
                // 인터럽트 발생하면, 코루틴 탈출
                if (IsInterrupted)
                    yield break;

                Target = GetNearestTarget(_monster.Detector.DetectedObstacles, closedSet);
                if (Target != null)
                {
                    closedSet.Add(Target);
                    _path = DefenseManager.Instance.FindPathToTarget(startPos, _monster.Stat.Size, Target);
                    if(_path != null)
                    {
                        // 경로가 있으면, 이동 상태 전환
                        ChangeState(MONSTER_STATE.Move);
                        yield break;
                    }
                    yield return null;
                }
            }
            while (Target != null);
        }

        // 2. 코어를 타겟으로 하여, 경로 탐색 진행
        Target = DefenseManager.Instance.Core.transform;
        _path = DefenseManager.Instance.FindPathToTarget(startPos, _monster.Stat.Size, Target);
        if(_path != null)
        {
            ChangeState(MONSTER_STATE.Move);
            yield break;
        }
        yield return null;

        // 3. 코어에서 가까운 거리 순으로 정렬된 리스트에서 해당 몬스터에게 가까운 순으로 순차적으로 경로 탐색 진행
        int count = 0;
        while(count < 1000)
        {
            // 인터럽트 발생하면, 코루틴 탈출
            if (IsInterrupted)
                yield break;

            // 타겟 리스트는 코어에서 체비쇼프 거리 기준 가까운 타겟 리스트를 순차적으로 가져옴
            List<Transform> targetList = DefenseManager.Instance.GetTargetList(count);
            if(targetList == null || targetList.Count == 0)
            {
                count++;
                continue;
            }

            // 타겟 리스트 중에서 탐색 진행이 안된 타겟 중에서 현재 몬스터와 가장 가까운 타겟을 탐색
            Target = GetNearestTarget(targetList, closedSet);
            if (Target == null)
            {
                count++;
                continue;
            }

            closedSet.Add(Target);
            _path = DefenseManager.Instance.FindPathToTarget(startPos, _monster.Stat.Size, Target);
            if (_path != null)
            {
                // 경로가 있으면, 이동 상태 전환
                ChangeState(MONSTER_STATE.Move);
                yield break;
            }
            yield return null;
        }

        // 4. 모든 타겟에 대한 경로가 존재하지 않으므로, invalid 상태 전환 - 아무것도 안함
        ChangeState(MONSTER_STATE.Invalid);
    }

    /// <summary>
    /// 목표 노드까지 이동
    /// </summary>
    private IEnumerator C_Move()
    {
        // 목표 노드까지 도착하지 않았으면, 이동 상태 유지
        while (Vector2.Distance(transform.position, _path.EndNode.WorldPos) > 0.01f)
        {
            // 코어가 부서지면, invalid 상태 전환 - 아무것도 안함
            if (DefenseManager.Instance.Core.IsDead)
            {
                ChangeState(InvalidState);
                yield break;
            }

            // 인터럽트 발생하면, 코루틴 탈출
            if (IsInterrupted)
            {
                yield break;
            }

            // 공격 범위 안에 타겟이 감지되면, 공격 상태 전환
            if (IsTargetInAttackRange())
            {
                ChangeState(MONSTER_STATE.Attack);
                yield break;
            }

            // 실제 오브젝트 이동
            switch (_monster.Stat.MoveType)
            {
                case MOVE_TYPE.Ground:
                    SetMonsterDirection(_path.CurNode.WorldPos);
                    break;

                case MOVE_TYPE.Air:
                    SetMonsterDirection(Target.position);
                    break;
            }
            _monster.Rigid.MovePosition(_monster.Rigid.position + _moveDir * _monster.Stat.Speed * Time.fixedDeltaTime);

            if (Vector2.Distance(transform.position, _path.CurNode.WorldPos) < 0.25f)
            {
                // 다음 노드로 이동할 예정이므로 도착한 노드의 해당 몬스터 해제
                _path.CurNode.monsterCount--;
                _path.Next();
            }

            yield return _waitFixedDeltaTime;
        }

        // 현재 목표 노드에는 도달할 수 없는데 도달했으므로, invalid 상태 전환 - 아무것도 안함
        ChangeState(MONSTER_STATE.Invalid);
    }

    /// <summary>
    /// 타겟에 대한 공격
    /// </summary>
    private IEnumerator C_Attack()
    {
        // 타겟이 살아있고 타겟이 공격 범위 안에 있으면, 공격 상태 유지
        while(_isTargetAlive && IsTargetInAttackRange())
        {
            // 코어가 부서지면, invalid 상태 전환 - 아무것도 안함
            if (DefenseManager.Instance.Core.IsDead)
            {
                ChangeState(InvalidState);
                yield break;
            }

            // 인터럽트 발생하면, 코루틴 탈출
            if (IsInterrupted)
                yield break;

            // 공격 애니메이션 실행
            SetMonsterDirection(Target.position);
            _animator?.SetTrigger(BaseMonster.ATTACK);

            // 공격 효과음 재생
            AudioManager.Instance.PlayMonsterSFX(_monster.Stat.MonsterName, "Attack");


            // 공격 타입에 따른 원/근거리 공격
            switch (_monster.Stat.AttackType)
            {
                case ATTACK_TYPE.Ranged:
                    // 투사체 프리팹이 없으면, invalid 상태 전환
                    if(_monster.Projectile == null)
                    {
                        ChangeState(MONSTER_STATE.Invalid);
                        yield break;
                    }

                    Projectile proj = Instantiate(_monster.Projectile).GetComponent<Projectile>();

                    ProjectileAttackSettings projAttackSettings = new()
                    {
                        attacker = proj.gameObject,
                        target = Target.gameObject,
                        damage = _monster.Stat.AttackPower,
                    };
                    ProjectileMovementSettings projMovementSettings = new()
                    {
                        firePosition = transform.position,
                        moveSpeed = 10f, //monster 쪽에 발사체 스피드 관련 정보 추가
                    };

                    proj.Init(projAttackSettings, projMovementSettings, new ProjectileMovement_StraightTarget(), new ProjectileAttack_Single());

                    break;

                case ATTACK_TYPE.Melee:
                    DamagedSystem.Instance.Send(new Damaged
                    {
                        Attacker = gameObject,
                        Victim = Target.gameObject,
                        Value = _monster.Stat.AttackPower,
                        IgnoreDefense = false
                    });

                    Debug.Log($"{Target.name}에게 {_monster.Stat.AttackPower}만큼의 데미지!");
                    break;
            }

            // Attack Per Sec 동안 기다림
            yield return Helper_Coroutine.WaitSeconds(_monster.Stat.AttackPerSec);
        }

        // 타겟이 없어지거나 멀어지면, 다시 탐색 상태 전환
        ChangeState(MONSTER_STATE.Explore);
    }

    /// <summary>
    /// 사망 애니메이션 & 풀링 반환
    /// </summary>
    private IEnumerator C_Dead()
    {
        _path?.ReleaseMonsterCount(); // 현재 몬스터를 경로 상에서 제외
        _monster.NotifyDeath(); // 자신을 감지하던 모든 타워에 몬스터 사망 알림
        _monster.Detector.DetectedObstacles.Clear(); // 몬스터가 감지한 타워 초기화
        AudioManager.Instance.PlayMonsterSFX(_monster.Stat.MonsterName, "Dead"); // 사망 효과음 재생

        yield return C_DeadAnimation();

        DefenseManager.Instance.MonsterSpawner.RemoveDeadMonster(_monster); // 몬스터 스포너에게 몬스터 사망 알림
        RewardSystem.Instance.GenerateRewards(_monster.GetId(), transform);
        PoolManager.Instance.ReturnToPool(_monster.GetId(), gameObject);
    }

    /// <summary>
    /// 점점 투명이 되는 사망 애니메이션
    /// </summary>
    private IEnumerator C_DeadAnimation()
    {
        float timer = 0f;
        while(timer <= _deadAnimDuration)
        {
            _monster.Spriter.color = _monster.Spriter.color.WithAlpha(Mathf.Clamp01(1f - timer / _deadAnimDuration));

            timer += Time.deltaTime;
            yield return null;
        }
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
    /// 타겟이 공격 범위 안에 있는지 체크하여 반환
    /// </summary>
    private bool IsTargetInAttackRange()
    {
        Vector2 direction = Target.position - transform.position;
        float attackRange = _monster.Stat.Size * 0.5f + _monster.Stat.AttackRange * 0.5f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, attackRange, _monster.ObstacleLayer);

        if(hits.Length == 0) return false;
        else
        {
            foreach (var hit in hits)
            {
                if (hit.transform == Target)
                {
                    return true;
                }
            }
        }
            
        return false;
    }

    /// <summary>
    /// 몬스터가 바라보는 방향 세팅
    /// </summary>
    private void SetMonsterDirection(Vector3 targetPos)
    {
        _moveDir = (targetPos - transform.position).normalized;
        _animator?.SetFloat(BaseMonster.X, _moveDir.x);
        _animator?.SetFloat(BaseMonster.Y, _moveDir.y);
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
    private void OnDrawGizmosSelected()
    {
        if(_path != null)
        {
            _path.DrawDebugGizmos();
        }
    }
}
