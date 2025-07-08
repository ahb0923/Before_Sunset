using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class ResourceSpawner<TData> : MonoBehaviour
{
    [Header("스폰 영역(직사각형)")]
    [SerializeField] private Vector2 spawnAreaCenter;
    [SerializeField] private Vector2 spawnAreaSize;

    [Header("스폰 개수")]
    [SerializeField] private int spawnCount = 20;

    [Header("중복 방지 반지름")]
    [SerializeField] private float overlapRadius = 1.0f;

    [Header("장애물 레이어 마스크")]
    [SerializeField] private LayerMask obstacleLayerMask;

    [Header("생성된 오브젝트 부모")]
    [SerializeField] private Transform parent;

    [Header("프리팹 경로")]
    [SerializeField] protected string prefabFolder = "Prefabs/Ore";  // 또는 "Prefabs/Jewel"

    [Header("프리팹 이름 접두사 (예: Ore, Jewel)")]
    [SerializeField] protected string prefabPrefix = "Ore";

    public Func<TData, int> GetId;
    public Func<TData, int> GetSpawnStage;
    public Func<TData, int> GetProbability;

    private List<TData> spawnableList = new();
    private List<Vector3> placedPositions = new();

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
            if (selected != null)
            {
                int id = GetId(selected);
                string path = $"{prefabFolder}/{prefabPrefix}_{id:D3}";
                GameObject prefab = Resources.Load<GameObject>(path);

                if (prefab == null)
                {
                    Debug.LogWarning($"Prefab not found at path: {path}");
                    continue;
                }

                GameObject obj = Instantiate(prefab, pos, Quaternion.identity, parent);
                placedPositions.Add(pos);
                placed++;
            }
        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        float x = UnityEngine.Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2f, spawnAreaCenter.x + spawnAreaSize.x / 2f);
        float y = UnityEngine.Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2f, spawnAreaCenter.y + spawnAreaSize.y / 2f);
        return new Vector3(x, y, 0f);
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
