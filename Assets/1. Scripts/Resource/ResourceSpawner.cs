using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSpawner<TData> : MonoBehaviour
{
    private Vector3 spawnAreaCenter3D;
    private Vector2 spawnAreaSize = new Vector2(57f, 31f);

    [Header("스폰 개수")]
    [SerializeField] private int spawnCount = 20;

    [Header("중복 방지 반지름")]
    [SerializeField] private float overlapRadius = 1.0f;

    [Header("장애물 레이어 마스크")]
    [SerializeField] private LayerMask obstacleLayerMask;

    public Func<TData, int> GetId;
    public Func<TData, int> GetSpawnStage;
    public Func<TData, int> GetProbability;

    private List<TData> spawnableList = new();
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

    public void SpawnResources(List<TData> dataList, int currentStage)
    {
        spawnableList.Clear();
        placedPositions.Clear();

        foreach (var data in dataList)
        {
            if (currentStage >= GetSpawnStage(data))
                spawnableList.Add(data);
        }

        if (spawnableList.Count == 0) return;

        int placed = 0;
        int attempts = 0;
        int maxAttempts = spawnCount * 10;

        while (placed < spawnCount && attempts < maxAttempts)
        {
            attempts++;
            Vector3 pos = GetRandomPositionInArea();

            if (Physics2D.OverlapCircle(pos, 0.1f, obstacleLayerMask)) continue;
            if (IsTooClose(pos)) continue;

            TData selected = GetRandomByProbability();
            if (selected == null) continue;

            int id = GetId(selected);

            GameObject obj = PoolManager.Instance.GetFromPool(id, pos);
            if (obj == null)
            {
                Debug.LogWarning($"Pool에서 오브젝트를 가져올 수 없습니다. ID: {id}");
                continue;
            }

            if (parentTransform != null)
                obj.transform.SetParent(parentTransform, false);

            obj.transform.position = pos;
            obj.GetComponent<IPoolable>()?.OnGetFromPool();

            placedPositions.Add(pos);
            placed++;
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

    private bool IsTooClose(Vector3 pos)
    {
        foreach (var otherPos in placedPositions)
        {
            if (Vector3.Distance(pos, otherPos) < overlapRadius)
                return true;
        }
        return false;
    }

    private TData GetRandomByProbability()
    {
        int totalWeight = spawnableList.Sum(d => GetProbability(d));
        if (totalWeight <= 0) return default;

        int rand = UnityEngine.Random.Range(0, totalWeight);
        int sum = 0;

        foreach (var data in spawnableList)
        {
            sum += GetProbability(data);
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

                stateComp.OnReturnToPool();
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
                resource.OnGetFromPool();
            }

            if (parentTransform != null)
                obj.transform.SetParent(parentTransform, false);

            obj.SetActive(true);
            spawnedObjects.Add(obj);
        }

        return spawnedObjects;
    }

    public GameObject SpawnSingle(TData data, Vector3 position)
    {
        int id = GetId(data);
        GameObject obj = PoolManager.Instance.GetFromPool(id, position);

        if (obj == null)
        {
            Debug.LogWarning($"[SpawnSingle] Pool에서 오브젝트를 가져오지 못함. ID: {id}");
            return null;
        }

        if (parentTransform != null)
            obj.transform.SetParent(parentTransform, false);

        obj.transform.position = position;

        if (obj.TryGetComponent<IResourceStateSavable>(out var resource))
        {
            if (resource is OreController ore)
            {
                ore.OnInstantiate();
            }
            else if (resource is JewelController jewel)
            {
                jewel.OnGetFromPool();
            }

            resource.OnGetFromPool();
        }

        obj.SetActive(true);
        return obj;
    }
}