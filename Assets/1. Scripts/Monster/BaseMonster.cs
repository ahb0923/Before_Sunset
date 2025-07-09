using UnityEngine;

public class BaseMonster : MonoBehaviour, IPoolable
{
    [SerializeField] private int _id;
    public int GetId() => _id;
    [SerializeField] private LayerMask _obstacleLayer;
    public LayerMask ObstacleLayer => _obstacleLayer;

    public SpriteRenderer Spriter { get; private set; }
    public MonsterAI Ai { get; private set; }
    public MonsterStatHandler Stat { get; private set; }
    public TargetDetector Detector { get; private set; }

    // 원거리 공격 몬스터만 투사체 받아옴
    [field: SerializeField] public GameObject Projectile { get; private set; }

    private void LateUpdate()
    {
        if (Spriter == null) return;
        RenderUtil.SetSortingOrderByY(Spriter);
    }

    /// <summary>
    /// 풀링에서 오브젝트 생성 시 단 1번 실행
    /// </summary>
    public void OnInstantiate()
    {
        Spriter = GetComponentInChildren<SpriteRenderer>();
        Ai = GetComponent<MonsterAI>();
        Stat = GetComponent<MonsterStatHandler>();
        Detector = GetComponentInChildren<TargetDetector>();

        Ai.Init(this);
        Stat.Init(this, _id);
        Detector.Init(this);
    }

    /// <summary>
    /// 풀링에서 가져올 때 호출
    /// </summary>
    public void OnGetFromPool()
    {
        Stat.SetFullHp();
        Ai.InitExploreState();
    }

    /// <summary>
    /// 풀링으로 반환할 때 호출
    /// </summary>
    public void OnReturnToPool()
    {
        Ai.ChangeState(MONSTER_STATE.Invalid);
        Detector.DetectedObstacles.Clear();
        DefenseManager.Instance.MonsterSpawner.RemoveDeadMonster(this);
    }

    public void SetMonsterTargeting(bool isAttackCore)
    {
        Detector.SetAttackCore(isAttackCore);
    }
}
