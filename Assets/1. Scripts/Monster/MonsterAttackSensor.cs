using UnityEngine;

public class MonsterAttackSensor : MonoBehaviour
{
    private BaseMonster _monster;
    private CircleCollider2D _collider;

    public void Init(BaseMonster monster)
    {
        _collider = GetComponent<CircleCollider2D>();
        _monster = monster;
    }

    /// <summary>
    /// 공격 센서 범위 설정
    /// </summary>
    public void SetSensorRange(int size, float range)
    {
        float halfSize = size * 0.5f;
        float halfRange = range * 0.5f;
        _collider.radius = halfSize + halfRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_monster.Ai.Target == null) return;

        // 타겟이 공격 범위 안에 들어오면 공격 상태 전환
        if(collision.gameObject == _monster.Ai.Target)
        {
            _monster.Ai.ChangeState(MONSTER_STATE.Attack);
        }
    }
}
