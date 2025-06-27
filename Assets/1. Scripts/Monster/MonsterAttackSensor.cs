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
    public void SetSensorRange(float range)
    {
        _collider.radius = range;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_monster.Ai.Target == null) return;

        if(collision.transform == _monster.Ai.Target)
        {
            _monster.Ai.ChangeState(MONSTER_STATE.Attack);
        }
    }
}
