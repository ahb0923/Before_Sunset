using System.Collections.Generic;
using UnityEngine;

public class BaseMonster : MonoBehaviour, IPoolable
{
    public static readonly int MOVE = Animator.StringToHash("Move");
    public static readonly int ATTACK = Animator.StringToHash("Attack");
    public static readonly int X = Animator.StringToHash("DirX");
    public static readonly int Y = Animator.StringToHash("DirY");

    [SerializeField] private int _id;
    public int GetId() => _id;
    [SerializeField] private LayerMask _obstacleLayer;
    public LayerMask ObstacleLayer => _obstacleLayer;

    public Rigidbody2D Rigid { get; private set; }
    public BoxCollider2D Collider { get; private set; }
    public MonsterAI Ai { get; private set; }
    public MonsterStatHandler Stat { get; private set; }
    public SpriteRenderer Spriter { get; private set; }
    public Animator Animator { get; private set; }
    public TargetDetector Detector { get; private set; }
    public EntityHpBar HpBar { get; private set; }

    // 원거리 공격 몬스터만 투사체 받아옴
    [field: SerializeField] public GameObject Projectile { get; private set; }

    public bool IsDead => Ai.CurState == MONSTER_STATE.Dead;

    // 자신을 타게팅하고 있는 타워들
    private HashSet<TowerAttackSensor> _registeredSensors = new();

    private void LateUpdate()
    {
        if (Spriter == null || Collider == null) return;

        float yPos = Collider.offset.y - Collider.size.y / 2;
        RenderUtil.SetSortingOrderByY(Spriter, yPos);
    }

    /// <summary>
    /// 풀링에서 오브젝트 생성 시 단 1번 실행
    /// </summary>
    public void OnInstantiate()
    {
        Rigid = GetComponent<Rigidbody2D>();
        Collider = GetComponent<BoxCollider2D>();
        Ai = GetComponent<MonsterAI>();
        Stat = GetComponent<MonsterStatHandler>();
        Spriter = GetComponentInChildren<SpriteRenderer>();
        Animator = GetComponentInChildren<Animator>();
        Detector = GetComponentInChildren<TargetDetector>();
        HpBar = GetComponentInChildren<EntityHpBar>();

        Ai.Init(this, Animator);
        Stat.Init(this, _id);
        Detector.Init(this);
        HpBar.Init(Stat.MaxHp);
    }

    /// <summary>
    /// 풀링에서 가져올 때 호출
    /// </summary>
    public void OnGetFromPool()
    {
        Stat.SetFullHp();
        HpBar.SetFullHpBar();
        Ai.InitExploreState();
    }

    /// <summary>
    /// 풀링으로 반환할 때 호출
    /// </summary>
    public void OnReturnToPool()
    {
        NotifyDeath();
        Ai.ChangeState(MONSTER_STATE.Invalid);
        Detector.DetectedObstacles.Clear();
        DefenseManager.Instance.MonsterSpawner.RemoveDeadMonster(this);
    }

    /// <summary>
    /// 몬스터 타겟팅 적용
    /// </summary>
    public void SetMonsterTargeting(bool isAttackCore)
    {
        Detector.SetAttackCore(isAttackCore);
    }



    /// <summary>
    /// 몬스터가 죽었을 때 자신을 감지하던 모든 타워에 알림
    /// </summary>
    public void NotifyDeath()
    {
        foreach (var sensor in _registeredSensors)
        {
            sensor.RemoveEnemy(gameObject); // 타워 측에서 hashSet 정리
        }
        _registeredSensors.Clear();
    }
    public void RegisterSensor(TowerAttackSensor sensor)
    {
        _registeredSensors.Add(sensor);
    }

    public void UnregisterSensor(TowerAttackSensor sensor)
    {
        _registeredSensors.Remove(sensor);
    }


}
