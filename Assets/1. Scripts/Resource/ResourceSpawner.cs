using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSpawner<TData> : MonoBehaviour
{
    private Vector3 spawnAreaCenter3D;
    private Vector2 spawnAreaSize = new Vector2(57f, 31f);

    [SerializeField] private int spawnMineralCount = 100;
    [SerializeField] private int spawnJewelCount = 1;
    [SerializeField] private float overlapRadius = 1.0f;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private LayerMask _spawnZoneLayer;

    private List<OreDatabase> spawnableList = new();
    private List<Vector3> placedPositions = new();

    private Transform parentTransform;

    public void SetParentTransform(Transform parent)
    {
        parentTransform = parent;
    }

    public Transform GetParentTransform()
    {
        return parentTransform;
    }

    public void SetSpawnArea(Vector3 center, Vector2 size)
    {
        spawnAreaCenter3D = center;
        spawnAreaSize = size;
    }

    public void SpawnResources(int currentStage)
    {
        var oreDatas = DataManager.Instance.OreData;
        spawnableList.Clear();
        placedPositions.Clear();

        foreach (var data in oreDatas.GetAllItems())
        {
            if (currentStage >= data.spawnStage)
                spawnableList.Add(data);
        }

        if (spawnableList.Count == 0) return;

        
        foreach(var data in spawnableList)
        {
            if(data.dropItemType == DROPITEM_TYPE.Jewel)
            {
                float probability = data.spawnProbability / 100f;

                if (UnityEngine.Random.value <= probability) 
                    TryPlaceSingle(data);
                else
                {
                    Debug.Log($"『{data.itemName}』확률에 패배! 소환 실패!");
                }
            }
        }

        int placed = 0;
        int attempts = 0;
        int maxAttempts = spawnMineralCount * 10;

        while (placed < spawnMineralCount && attempts < maxAttempts)
        {
            attempts++;
            Vector3 pos = GetRandomPositionInArea();

            if (!IsValidSpawnPosition(pos)) continue;
            if (IsTooClose(pos)) continue;

            OreDatabase selected = GetRandomByProbability();
            if (selected == null || selected.dropItemType == DROPITEM_TYPE.Jewel) continue;

            if (TryPlace(selected, pos))
                placed++;
        }
    }
    /// <summary>
    /// 실제 광물 소환
    /// </summary>
    /// <param name="data"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool TryPlace(OreDatabase data, Vector3 pos)
    {
        int id = data.id;

        /*
        GameObject obj = PoolManager.Instance.GetFromPool(id, pos);
        if (obj == null)
        {
            Debug.LogWarning($"Pool에서 오브젝트를 가져올 수 없습니다. ID: {id}");
            return false;
        }

        if (parentTransform != null)
            obj.transform.SetParent(parentTransform, false);
        obj.transform.position = pos;
        */

        if(parentTransform != null)
        {
            GameObject obj = PoolManager.Instance.GetFromPool(id, pos, parentTransform);
            if (obj == null)
            {
                Debug.LogWarning($"Pool에서 오브젝트를 가져올 수 없습니다. ID: {id}");
                return false;
            }
        }

        placedPositions.Add(pos);
        return true;
    }
    /// <summary>
    /// Jewel 스폰시에만 사용 TryPlace 가기 전 작업
    /// </summary>
    /// <param name="data"></param>
    private void TryPlaceSingle(OreDatabase data)
    {
        Debug.Log($"{data.itemName} / {data.dropItemType} 이거 소환됨 ㅇㅇ");

        for (int i = 0; i < spawnJewelCount; i++)
        {
            Vector3 pos = GetRandomPositionInArea();

            if (!IsValidSpawnPosition(pos)) continue;
            if (Physics2D.OverlapCircle(pos, 0.1f, obstacleLayerMask)) continue;
            if (IsTooClose(pos)) continue;

            TryPlace(data, pos);
            break;
        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        float x = UnityEngine.Random.Range(
            spawnAreaCenter3D.x - spawnAreaSize.x / 2f,
            spawnAreaCenter3D.x + spawnAreaSize.x / 2f
        );
        float y = UnityEngine.Random.Range(
            spawnAreaCenter3D.y - spawnAreaSize.y / 2f,
            spawnAreaCenter3D.y + spawnAreaSize.y / 2f
        );
        float z = spawnAreaCenter3D.z;

        // 타일 중심으로 스냅
        x = Mathf.Floor(x) + 0.5f;
        y = Mathf.Floor(y) + 0.5f;

        return new Vector3(x, y, z);
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        // 스폰 가능 레이어
        Collider2D spawnZoneCollider = Physics2D.OverlapCircle(position, 0.3f, _spawnZoneLayer);
        if (spawnZoneCollider == null) return false;

        // 스폰 불가능 레이어
        Collider2D obstacleCollider = Physics2D.OverlapCircle(position, 0.3f, obstacleLayerMask);
        if (obstacleCollider != null) return false;

        return true;
    }

    private bool IsTooClose(Vector3 pos)
    {
        foreach (var otherPos in placedPositions)
        {
            if (Vector3.Distance(pos, otherPos) < overlapRadius)
                return true;
        }
        return false;
    }

    private OreDatabase GetRandomByProbability()
    {
        int totalWeight = spawnableList.Sum(d => d.spawnProbability);
        if (totalWeight <= 0) return default;

        int rand = UnityEngine.Random.Range(0, totalWeight);
        int sum = 0;

        foreach (var data in spawnableList)
        {
            sum += data.spawnProbability;
            if (rand < sum)
                return data;
        }

        return default;
    }

    public List<ResourceState> SaveCurrentStates(bool inactive = true)
    {
        var savedStates = new List<ResourceState>();
        var parent = GetParentTransform();
        if (parent == null) return savedStates;

        List<Transform> toReturn = new();

        for (int i = 0; i < parent.childCount; i++)
        {
            var obj = parent.GetChild(i).gameObject;
            var stateComp = obj.GetComponent<IResourceStateSavable>();

            if (stateComp != null)
            {
                // 위치 변환 없이 저장
                savedStates.Add(stateComp.SaveState());
            }

            toReturn.Add(parent.GetChild(i));
        }

        if (inactive)
            InactiveResources(toReturn);

        return savedStates;
    }

    /// <summary>
    /// 저장이 완료된 리소스 오브젝트 비활성화
    /// </summary>
    private void InactiveResources(List<Transform> savedResources)
    {
        foreach (var tr in savedResources)
        {
            var poolable = tr.GetComponent<IPoolable>();
            if (poolable != null)
            {
                PoolManager.Instance.ReturnToPool(poolable.GetId(), tr.gameObject);
            }
            else
            {
                Debug.LogWarning($"[ReturnToPool] IPoolable 컴포넌트가 없습니다: {tr.name}");
            }
        }
    }

    public List<GameObject> SpawnFromSavedStates(List<ResourceState> savedStates)
    {
        List<GameObject> spawnedObjects = new List<GameObject>();

        foreach (var state in savedStates)
        {
            // 저장된 위치 그대로 사용
            GameObject obj = PoolManager.Instance.GetFromPool(state.Id, state.Position);
            if (obj == null)
            {
                Debug.LogWarning($"풀에서 꺼내기 실패 ID: {state.Id}");
                continue;
            }

            if (obj.TryGetComponent<IResourceStateSavable>(out var resource))
            {
                resource.LoadState(state);
            }

            if (parentTransform != null)
                obj.transform.SetParent(parentTransform, false);

            obj.SetActive(true);
            spawnedObjects.Add(obj);
        }

        return spawnedObjects;
    }
}