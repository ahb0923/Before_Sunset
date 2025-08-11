using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    private BaseMonster _monster;
    public List<Transform> DetectedObstacles { get; private set; }

    /// <summary>
    /// 타겟 감지기 초기화
    /// </summary>
    public void Init(BaseMonster monster)
    {
        _monster = monster;
        DetectedObstacles = new List<Transform>();
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
