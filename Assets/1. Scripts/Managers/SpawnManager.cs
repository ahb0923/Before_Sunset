using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour, ISaveable
{
    private OreSpawner oreSpawner;
    private JewelSpawner jewelSpawner;
    private OreDataHandler oreHandler;
    private JewelDataHandler jewelHandler;

    private Vector3 currentMapPosition = Vector3.zero;
    private int currentMapIndex = -1;

    private Dictionary<int, List<GameObject>> mapResources = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, List<ResourceState>> _mapResourceStates = new();

    private void Start()
    {
        oreSpawner = Helper_Component.FindChildComponent<OreSpawner>(this.transform, "OreSpawner");
        jewelSpawner = Helper_Component.FindChildComponent<JewelSpawner>(this.transform, "JewelSpawner");

        StartCoroutine(WaitForDataManagerInit());
    }
    private IEnumerator WaitForDataManagerInit()
    {
        while (!DataManagerReady())
            yield return null;

        oreHandler = DataManager.Instance.OreData;
        jewelHandler = DataManager.Instance.JewelData;
    }

    private bool DataManagerReady()
    {
        var task = DataManager.Instance.InitCheck();
        return task.IsCompleted && DataManager.Instance.OreData != null && DataManager.Instance.JewelData != null;
    }

    private Transform GetContainer(string containerName)
    {
        var container = GameObject.Find(containerName);
        if (container == null)
            Debug.LogWarning($"컨테이너 {containerName}를 찾을 수 없습니다.");
        return container?.transform;
    }

    public void OnMapChanged(Vector3 mapPosition, int mapIndex, Vector2[] spawnAreas)
    {
        if (spawnAreas == null || spawnAreas.Length < 2)
        {
            Debug.LogError("스폰 영역 정보가 부족합니다.");
            return;
        }

        // 현재 맵 상태 저장
        if (currentMapIndex != -1)
        {
            // 1) 상태 저장
            List<ResourceState> saved = new();
            saved.AddRange(oreSpawner.SaveCurrentStates());
            saved.AddRange(jewelSpawner.SaveCurrentStates());
            _mapResourceStates[currentMapIndex] = saved;

            // 2) 이전 맵 자원들 풀에 반환
            if (mapResources.TryGetValue(currentMapIndex, out var oldResources))
            {
                foreach (var obj in oldResources)
                {
                    if (obj == null) continue;
                    if (obj.TryGetComponent<IResourceStateSavable>(out var resource))
                    {
                        resource.OnReturnToPool();
                    }
                    PoolManager.Instance.ReturnToPool(obj.GetComponent<IPoolable>().GetId(), obj);
                }
                oldResources.Clear();
            }
        }

        currentMapIndex = mapIndex;
        currentMapPosition = mapPosition;
        SetMapPositionAndArea(mapPosition, spawnAreas[0], spawnAreas[1]);

        if (currentMapIndex == 0)
        {
            return;
        }

        var oreContainer = GetContainer("OreContainer");
        var jewelContainer = GetContainer("JewelContainer");

        oreSpawner.SetParentTransform(oreContainer);
        jewelSpawner.SetParentTransform(jewelContainer);

        oreContainer.transform.position = Vector3.zero;
        jewelContainer.transform.position = Vector3.zero;

        if (_mapResourceStates.TryGetValue(currentMapIndex, out var savedStates))
        {
            List<GameObject> newResources = new List<GameObject>();

            newResources.AddRange(oreSpawner.SpawnFromSavedStates(savedStates.Where(s => s.Id < 1000).ToList()));
            newResources.AddRange(jewelSpawner.SpawnFromSavedStates(savedStates.Where(s => s.Id >= 1000).ToList()));

            mapResources[currentMapIndex] = newResources;
        }
        else
        {
            SpawnAllAndStore();
        }
    }

    private void SpawnAllAndStore()
    {
        if (oreHandler == null || jewelHandler == null)
        {
            Debug.LogWarning("SpawnManager: 데이터 핸들러가 세팅되지 않았습니다.");
            return;
        }

        List<GameObject> spawnedObjects = new List<GameObject>();

        List<OreDatabase> oreList = oreHandler.GetAllItems();
        List<JewelDatabase> jewelList = jewelHandler.GetAllItems();

        if (oreSpawner != null && oreList != null)
        {
            oreSpawner.SpawnResources(oreList, TimeManager.Instance.Stage);
            spawnedObjects.AddRange(GetChildrenGameObjects(oreSpawner.GetParentTransform()));
        }

        if (jewelSpawner != null && jewelList != null)
        {
            jewelSpawner.SpawnResources(jewelList, TimeManager.Instance.Stage);
            spawnedObjects.AddRange(GetChildrenGameObjects(jewelSpawner.GetParentTransform()));
        }

        mapResources[currentMapIndex] = spawnedObjects;
    }

    private List<GameObject> GetChildrenGameObjects(Transform parent)
    {
        List<GameObject> list = new List<GameObject>();
        if (parent == null) return list;

        for (int i = 0; i < parent.childCount; i++)
            list.Add(parent.GetChild(i).gameObject);
        return list;
    }

    public void OnStageChanged()
    {
        ClearAll();
        mapResources.Clear();
    }

    private void ClearAll()
    {
        ClearChildren(oreSpawner?.GetParentTransform());
        ClearChildren(jewelSpawner?.GetParentTransform());
        mapResources.Clear();
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    public void SetMapResourcesActive(int mapIndex, bool active)
    {
        if (mapResources.TryGetValue(mapIndex, out var resources))
        {
            foreach (var obj in resources)
                obj.SetActive(active);
        }
    }

    public void SetMapPositionAndArea(Vector3 mapPosition, Vector2 oreAreaSize, Vector2 jewelAreaSize)
    {
        currentMapPosition = mapPosition;
        oreSpawner?.SetSpawnArea(currentMapPosition, oreAreaSize);
        jewelSpawner?.SetSpawnArea(currentMapPosition, jewelAreaSize);
    }

    /// <summary>
    /// 광산에 스폰된 광석과 쥬얼 데이터 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        foreach(var key in _mapResourceStates.Keys)
        {
            MineSaveData mineData = new MineSaveData();

            mineData.mapId = key;
            foreach(ResourceState resource in _mapResourceStates[key])
            {
                if(!resource.IsMined)
                    mineData.resources.Add(new ResourceSaveData(resource.Id, resource.Position, resource.HP));
            }

            data.spawnedMines.Add(mineData);
        }
    }

    /// <summary>
    /// 광산에 스폰된 광석과 쥬얼 데이터 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        _mapResourceStates.Clear();

        foreach(MineSaveData mineData in data.spawnedMines)
        {
            List<ResourceState> resourceStates = new List<ResourceState>();

            foreach(ResourceSaveData resourceData in mineData.resources)
            {
                ResourceState resourceState = new ResourceState()
                {
                    Id = resourceData.resourceId,
                    Position = resourceData.position,
                    HP = resourceData.curHp,
                    IsMined = false
                };

                resourceStates.Add(resourceState);
            }

            _mapResourceStates[mineData.mapId] = resourceStates;
        }
    }
}