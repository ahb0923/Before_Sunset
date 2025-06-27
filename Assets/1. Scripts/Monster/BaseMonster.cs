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
        Sensor = GetComponentInChildren<MonsterAttackSensor>(true);

        Ai.Init(this, MonsterSpawner.Instance.Core);
        Stat.Init(this);
        Sensor.Init(this);
    }

    public void OnGetFromPool()
    {
        Stat.CurHp = Stat.MaxHp;
        Sensor.SetSensorRange(Stat.AttackRange);
        Ai.InitExploreState();
    }

    public void OnReturnToPool()
    {
        
    }
}
