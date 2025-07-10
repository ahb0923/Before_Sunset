using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    private BaseMonster _monster;
    private CircleCollider2D _collider;

    public List<Transform> DetectedObstacles { get; private set; }

    public void Init(BaseMonster monster)
    {
        _monster = monster;
        _collider = GetComponent<CircleCollider2D>();
        
        DetectedObstacles = new List<Transform>();
    }

    public void SetAttackCore(bool isAttackCore)
    {
        _collider.enabled = !isAttackCore;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_monster == null) return;

        if (((1 << collision.gameObject.layer) & _monster.ObstacleLayer) != 0)
        {
            DetectedObstacles.Add(collision.transform);
            _monster.Ai.ChangeState(MONSTER_STATE.Explore);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_monster == null) return;

        if (((1 << collision.gameObject.layer) & _monster.ObstacleLayer) != 0)
        {
            if (DetectedObstacles.Contains(collision.transform))
            {
                DetectedObstacles.Remove(collision.transform);
            }
        }
    }
}
