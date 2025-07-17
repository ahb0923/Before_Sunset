using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private OreSpawner oreSpawner;
    private JewelSpawner jewelSpawner;
    private OreDataHandler oreHandler;
    private JewelDataHandler jewelHandler;

    private Vector3 currentMapPosition = Vector3.zero;
    private int currentMapIndex = -1;

    private Dictionary<int, List<GameObject>> mapResources = new Dictionary<int, List<GameObject>>();

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

        if (currentMapIndex == mapIndex)
        {
            // 이미 스폰된 오브젝트 활성화
            SetMapResourcesActive(mapIndex, true);
            return;
        }

        // 이전 맵 오브젝트 비활성화
        if (currentMapIndex != -1)
            SetMapResourcesActive(currentMapIndex, false);

        currentMapIndex = mapIndex;
        currentMapPosition = mapPosition;

        SetMapPositionAndArea(mapPosition, spawnAreas[0], spawnAreas[1]);

        if (currentMapIndex == 0)
        {
            Debug.Log("기본맵이라 자원 스폰하지 않음");
            return;
        }

        var oreContainer = GetContainer("OreContainer");
        var jewelContainer = GetContainer("JewelContainer");

        oreSpawner.SetParentTransform(oreContainer);
        jewelSpawner.SetParentTransform(jewelContainer);

        if (mapResources.ContainsKey(currentMapIndex))
        {
            // 기존 스폰 오브젝트 재활성화
            SetMapResourcesActive(currentMapIndex, true);
        }
        else
        {
            // 새로 스폰 후 저장
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
}