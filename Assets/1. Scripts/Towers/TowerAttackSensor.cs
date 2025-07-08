using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackSensor : MonoBehaviour
{
    private BaseTower _tower;

    private HashSet<GameObject> detectedEnemies = new();
    public HashSet<GameObject> DetectedEnemies { get { return detectedEnemies; } }

    private GameObject _currentTarget;
    public GameObject CurrentTarget => _currentTarget;



    [Header("[ 에디터 할당 ]")] 
    [SerializeField] private LayerMask enemyLayerMask;

    public void Init(BaseTower basetower)
    {
        _tower = basetower;
    }

    public bool HasDetectedEnemy() => detectedEnemies.Count > 0;

    public void ScanInitialEnemies()
    {
        List<Collider2D> results = new();

        // 충돌 감지용 구조체
        ContactFilter2D filter = new()
        {
            useLayerMask = true,
            layerMask = enemyLayerMask,
            useTriggers = true          //트리거(IsTrigger) Collider도 포함해서 검사할지 여부
        };

        int count = GetComponent<Collider2D>().OverlapCollider(filter, results);

        for (int i = 0; i < count; i++)
        {
            var hit = results[i];
            if (hit == null || !hit.gameObject.activeSelf) continue;

            if (detectedEnemies.Add(hit.gameObject))
            {
                Debug.Log($"[초기 스캔] 감지된 적: {hit.name}");
            }
        }
    }
    /// <summary>
    /// detectedEnemies 해시셋에 적 저장
    /// </summary>
    /// <param name="enemy"></param>
    public void RegistEnemy(GameObject enemy)
    {
        if (detectedEnemies.Add(enemy))
        {
            Debug.Log($"적 진입: {enemy.name} | 총 {detectedEnemies.Count}명 감지됨");
            if (detectedEnemies.Count == 1)
                _tower.ai.SetState(TOWER_STATE.Attack);
        }
    }

    /// <summary>
    /// detectedEnemies 해시셋에서 적 제거
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(GameObject enemy)
    {
        if (detectedEnemies.Remove(enemy))
        {
            Debug.Log($"[센서] 적 제거됨: {enemy.name} | 남은 {detectedEnemies.Count}명");
            if (enemy == _currentTarget)
            {
                Debug.Log("[센서] 현재 타겟 제거 → 새 타겟 찾기");
                RefreshTarget();
            }
            if (detectedEnemies.Count == 0)
                _tower.ai.SetState(TOWER_STATE.Idle);
        }
    }

    /// <summary>
    /// detectedEnemies 해시셋에 등록된 가장 근접한 적을 찾는 메서드
    /// </summary>
    /// <returns></returns>
    public GameObject NearestTarget()
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;
        Vector3 origin = transform.position;

        foreach (var enemy in detectedEnemies)
        {
            // 검사하는 타이밍에 맞춰서 다른 타워가 해당 몬스터를 부숴버릴 경우에 대비
            if (enemy == null) continue;

            float dist = Vector3.Distance(origin, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }
    /// <summary>
    /// detectedEnemies 해시셋에 등록된 가장 멀리 있는 적을 찾는 메서드
    /// </summary>
    /// <returns></returns>
    public GameObject FurthestTarget()
    {
        GameObject furthest = null;
        float maxDist = float.MinValue;
        Vector3 origin = transform.position;

        foreach (var enemy in detectedEnemies)
        {
            // 검사하는 타이밍에 맞춰서 다른 타워가 해당 몬스터를 부숴버릴 경우에 대비
            if (enemy == null) continue;

            float dist = Vector3.Distance(origin, enemy.transform.position);
            if (dist > maxDist)
            {
                maxDist = dist;
                furthest = enemy;
            }
        }
        return furthest;
    }
    /// <summary>
    ///  detectedEnemies 해시셋에 등록된 체력이 가장 높은 적을 찾는 메서드
    /// </summary>
    /// <returns></returns>
    public GameObject HighHpTarget()
    {
        GameObject highHp = null;
        float maxHp = float.MinValue;

        foreach (var enemy in detectedEnemies)
        {
            if (enemy == null) continue;

            var stat = enemy.GetComponent<MonsterStatHandler>();
            if (stat == null || !enemy.activeSelf) continue;

            if (stat.CurHp > maxHp)
            {
                maxHp = stat.CurHp;
                highHp = enemy;
            }
        }
        return highHp;
    }


    /// <summary>
    /// 현재 타겟을 새로이 설정할때 쓰는 메서드<br/>
    /// 찾는 타겟이 없다면 타워의 상태를 Idle로 변경
    /// </summary>
    public void RefreshTarget()
    {
        _currentTarget = NearestTarget();

        if (_currentTarget != null)
        {
            Debug.Log($"[센서] 새 타겟 설정: {_currentTarget.name}");
            _tower.ai.SetState(TOWER_STATE.Attack);
        }
        else
        {
            Debug.Log("[센서] 타겟 없음 → 대기 상태");
            _tower.ai.SetState(TOWER_STATE.Idle);
        }
    }

    /// <summary>
    /// 현재 타겟이 범위를 벗어났을 경우(hashSet에서 제거되었을 경우) 새로운 적을 탐색
    /// </summary>
    public void CheckTargetValid()
    {
        if (_currentTarget == null || !_currentTarget.activeSelf)
        {
            RefreshTarget();  // 가장 가까운 적으로 다시 설정
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_tower.ai.CurState == TOWER_STATE.Destroy)
            return;

        if (((1 << other.gameObject.layer) & enemyLayerMask) != 0)
        {
            if (detectedEnemies.Add(other.gameObject))
            {
                Debug.Log($"적 진입: {other.name} | 총 {detectedEnemies.Count}명 감지됨");
                if (detectedEnemies.Count == 1)
                {
                    _tower.ai.SetState(TOWER_STATE.Attack);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_tower.ai.CurState == TOWER_STATE.Destroy)
            return;

        if (((1 << other.gameObject.layer) & enemyLayerMask) != 0)
        {
            if (detectedEnemies.Remove(other.gameObject))
            {
                 Debug.Log($"적 이탈: {other.name} | 남은 {detectedEnemies.Count}명");
                if (other.gameObject == _currentTarget)
                {
                    Debug.Log($"[센서] 현재 타겟 이탈: {other.name} → 타겟 갱신");
                    RefreshTarget();
                }
                if (detectedEnemies.Count == 0)
                {
                    _tower.ai.SetState(TOWER_STATE.Idle);
                }
            }
        }
    }
}
