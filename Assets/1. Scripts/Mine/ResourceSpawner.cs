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
            obj.SetActive(true);
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
}