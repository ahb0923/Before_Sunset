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
    private List<Vector3> spawnableTiles = new List<Vector3>();
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

    /// <summary>
    /// 자원 스폰
    /// </summary>
    /// <param name="currentStage"></param>
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

        GetSpawnTiles();
        
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

            if (pos == Vector3.zero) break;

            if (IsTooClose(pos)) continue;

            OreDatabase selected = GetRandomByProbability();
            if (selected == null || selected.dropItemType == DROPITEM_TYPE.Jewel) continue;

            if (TryPlace(selected, pos))
            {
                placed++;
                spawnableTiles.Remove(pos);
            }
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
        PoolManager.Instance.GetFromPool(id, pos, parentTransform);
        placedPositions.Add(pos);
        return true;
    }

    /// <summary>
    /// Jewel 스폰시에만 사용 TryPlace 가기 전 작업
    /// </summary>
    /// <param name="data"></param>
    private void TryPlaceSingle(OreDatabase data)
    {
        for (int i = 0; i < spawnJewelCount; i++)
        {
            Vector3 pos = GetRandomPositionInArea();
            if (pos == Vector3.zero)
            {
                break;
            }
            if (IsTooClose(pos)) 
            {
                continue; 
            }
            if (TryPlace(data, pos))
            {
                spawnableTiles.Remove(pos);
                break;
            }
        }
    }

    /// <summary>
    /// 스폰 영역 먼저 세팅
    /// </summary>
    public void GetSpawnTiles()
    {
        spawnableTiles.Clear();

        // 스폰 영역 계산
        float minX = spawnAreaCenter3D.x - spawnAreaSize.x / 2f;
        float maxX = spawnAreaCenter3D.x + spawnAreaSize.x / 2f;
        float minY = spawnAreaCenter3D.y - spawnAreaSize.y / 2f;
        float maxY = spawnAreaCenter3D.y + spawnAreaSize.y / 2f;

        // 타일 위치 찾기
        for (float x = minX; x < maxX; x++)
        {
            for (float y = minY; y < maxY; y++)
            {
                Vector3 pos = new Vector3(Mathf.Floor(x) + 0.5f, Mathf.Floor(y) + 0.5f, spawnAreaCenter3D.z);
                if (IsValidSpawnPosition(pos))
                {
                    spawnableTiles.Add(pos);
                }
            }
        }
    }

    /// <summary>
    /// 스폰 영역 내 랜덤 위치
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPositionInArea()
    {
        if (spawnableTiles.Count == 0)
        {
            return Vector3.zero;
        }
        int randomIndex = UnityEngine.Random.Range(0, spawnableTiles.Count);
        return spawnableTiles[randomIndex];
    }

    /// <summary>
    /// 스폰존 레이어 찾기
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsValidSpawnPosition(Vector3 position)
    {
        // 스폰 가능 레이어
        Collider2D spawnZoneCollider = Physics2D.OverlapCircle(position, 0.3f, _spawnZoneLayer);
        if (spawnZoneCollider == null) return false;

        // 스폰 불가능 레이어
        //Collider2D obstacleCollider = Physics2D.OverlapCircle(position, 0.3f, obstacleLayerMask);
        //if (obstacleCollider != null) return false;

        return true;
    }

    // 없어도될듯 아마도..
    private bool IsTooClose(Vector3 pos)
    {
        foreach (var otherPos in placedPositions)
        {
            if (Vector3.Distance(pos, otherPos) < overlapRadius)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 광석 가중치 기반 랜덤 뽑기
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 배치된 자원들 저장 (맵 그대로 꺼낼 때 사용)
    /// </summary>
    /// <param name="inactive"></param>
    /// <returns></returns>
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
    /// 저장이 완료된 자원 풀로 반환
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

    /// <summary>
    /// 저장된 상태 불러오기
    /// </summary>
    /// <param name="savedStates"></param>
    /// <returns></returns>
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