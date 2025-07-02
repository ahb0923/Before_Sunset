using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackSensor : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleLayer;

    private BaseMonster _monster;
    private CircleCollider2D _collider;

    public HashSet<Transform> DetectedObstacles = new HashSet<Transform>();

    public void Init(BaseMonster monster, int size, float range)
    {
        _collider = GetComponent<CircleCollider2D>();
        _monster = monster;

        float halfSize = size * 0.5f;
        float halfRange = range * 0.5f;
        _collider.radius = halfSize + halfRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(((1 << collision.gameObject.layer) & _obstacleLayer) != 0)
        {
            DetectedObstacles.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _obstacleLayer) != 0)
        {
            if(DetectedObstacles.Contains(collision.transform))
                DetectedObstacles.Remove(collision.transform);
        }
    }
}
