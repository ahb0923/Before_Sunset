using UnityEngine;

public class BaseMonster : MonoBehaviour, IPoolable
{
    public MonsterAI Ai { get; private set; }
    public MonsterStatHandler Stat { get; private set; }
    public MonsterAttackSensor Sensor { get; private set; }

    [SerializeField] private GameObject projectile;

    private void Awake()
    {
        Ai = GetComponent<MonsterAI>();
        Stat = GetComponent<MonsterStatHandler>();
        Sensor = GetComponentInChildren<MonsterAttackSensor>();

        Ai.Init(this, MonsterSpawner.Instance.Core);
        Stat.Init(this);
        Sensor.Init(this);
    }

    /// <summary>
    /// 풀링에서 가져올 때 호출
    /// </summary>
    public void OnGetFromPool()
    {
        Stat.CurHp = Stat.MaxHp;
        Sensor.SetSensorRange(Stat.Size, Stat.AttackRange);
        Ai.InitExploreState();
    }

    /// <summary>
    /// 풀링으로 반환할 때 호출
    /// </summary>
    public void OnReturnToPool()
    {
        
    }
}
