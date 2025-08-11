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
            Doing = C_Explore,
        });

        AddState(MONSTER_STATE.Move, new StateElem
        {
            Entered = () => 
            {
                _animator?.SetBool(BaseMonster.MOVE, true);
            },
            Doing = C_Move,
            Exited = () =>
            {
                _animator?.SetBool(BaseMonster.MOVE, false);
            },
        });

        AddState(MONSTER_STATE.Attack, new StateElem
        {
            Doing = C_Attack,
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

    /// <summary>
    /// 공격 상태에서는 외부 전환이 안 되도록 막음
    /// </summary>
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
        
        // 공격 상태에서 갑자기 타겟을 바꾸어서 공격하는 것 방지
        if(PrevState == MONSTER_STATE.Attack && _isTargetAlive)
        {
            ChangeState(MONSTER_STATE.Attack);
            yield break;
        }

        // 시작 위치 설정 & 초기화
        Vector3 startPos = transform.position;
        if(_path != null)
        {
            // 새로운 경로를 받아올 예정이므로, 현재 경로 노드에서 해당 몬스터 제외
            _path.ReleaseMonsterCount();
            _path = null;
        }

        // 1. 탐색 범위 내에 타겟 리스트가 존재하면, 가장 가까운 타겟부터 순차적으로 경로 탐색
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
                    // 공중 타입이면, 타겟이 설정되었으므로 이동 상태 전환
                    if (_monster.Stat.MoveType == MOVE_TYPE.Air)
                    {
                        ChangeState(MONSTER_STATE.Move);
                        yield break;
                    }

                    closedSet.Add(Target);

                    _path = DefenseManager.Instance.FindPathToTarget(startPos, 1, Target);

                    // 경로가 있으면, 이동 상태 전환
                    if(_path != null)
                    {
                        ChangeState(MONSTER_STATE.Move);
                        yield break;
                    }
                }
            }
            while (Target != null);
        }

        // 2. 코어를 타겟으로 하여, 경로 탐색 진행
        Target = DefenseManager.Instance.Core.transform;
        _path = DefenseManager.Instance.FindPathToTarget(startPos, 1, Target);
        ChangeState(MONSTER_STATE.Move);
    }

    /// <summary>
    /// 목표 노드까지 이동
    /// </summary>
    private IEnumerator C_Move()
    {
        // 다시 뒤로 돌아가는 현상 해결을 위해, 다음 노드로 이동
        if (_path != null) _path.Next();

        // 목표 노드까지 도착하지 않았으면, 이동 상태 유지
        while (Vector2.Distance(transform.position, Target.transform.position) > 0.01f)
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

            // 타겟이 사라지면, 탐색 상태 전환
            if (!_isTargetAlive)
            {
                ChangeState(MONSTER_STATE.Explore);
                yield break;
            }

            // 공격 범위 안에 타겟이 감지되면, 공격 상태 전환
            if (IsTargetInAttackRange())
            {
                ChangeState(MONSTER_STATE.Attack);
                yield break;
            }

            // 실제 오브젝트 이동
            if(_path != null)
            {
                // 경로가 존재하면, 경로를 따라 이동
                SetMonsterDirection(_path.CurNode.WorldPos);
                _monster.Rigid.MovePosition(_monster.Rigid.position + _moveDir * _monster.Stat.Speed * Time.fixedDeltaTime);

                if (Vector2.Distance(transform.position, _path.CurNode.WorldPos) < 0.05f) _path.Next();
            }
            else
            {
                // 경로가 없으면, 타겟을 향해 이동
                SetMonsterDirection(Target.position);
                _monster.Rigid.MovePosition(_monster.Rigid.position + _moveDir * _monster.Stat.Speed * Time.fixedDeltaTime);
            }

            yield return _waitFixedDeltaTime;
        }

        // 현재 목표 노드에는 도달할 수 없는데 도달했으므로, 탐색 상태 전환
        ChangeState(MONSTER_STATE.Explore);
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
            if (closed.Contains(target) || !IsTargetInCoreDirection(target)) continue;

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
    /// 타겟이 코어 방향에 있는지 확인
    /// </summary>
    private bool IsTargetInCoreDirection(Transform target)
    {
        Vector3 coreDir = DefenseManager.Instance.Core.transform.position - transform.position;
        Vector3 targetDir = target.position - transform.position;

        // 내적을 통한 코사인 값 구하기
        float dotProduct = Vector3.Dot(coreDir.normalized, targetDir.normalized);
        return dotProduct > 0;
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
