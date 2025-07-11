using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("스포너 컴포넌트")]
    [SerializeField] private OreSpawner oreSpawner;
    [SerializeField] private JewelSpawner jewelSpawner;

    [Header("현재 스테이지")]
    [SerializeField] private int currentStage = 1;

    private Vector3 currentMapPosition = Vector3.zero;

    [Header("기본 스폰 영역 크기")]
    [SerializeField] private Vector2 defaultOreSpawnAreaSize = new Vector2(30f, 30f);
    [SerializeField] private Vector2 defaultJewelSpawnAreaSize = new Vector2(20f, 20f);

    private OreDataHandler oreHandler = new OreDataHandler();
    private JewelDataHandler jewelHandler = new JewelDataHandler();

    private bool oreDataLoaded = false;
    private bool jewelDataLoaded = false;

    private int currentMapIndex = -1; // -1은 초기 상태 (맵 미지정)

    // 맵 인덱스별로 스폰된 자원 저장
    private Dictionary<int, List<GameObject>> mapResources = new Dictionary<int, List<GameObject>>();

    private async void Start()
    {
        await LoadAllDataAsync();
        SetMapPositionAndArea(currentMapPosition, defaultOreSpawnAreaSize, defaultJewelSpawnAreaSize);

        // 기본 맵 인덱스가 0이라면 스폰 안 하고, 필요시 다른 맵부터 스폰 가능
        currentMapIndex = 0;
    }

    private async Task LoadAllDataAsync()
    {
        if (!oreDataLoaded)
        {
            await oreHandler.LoadAsyncLocal();
            oreDataLoaded = true;
        }

        if (!jewelDataLoaded)
        {
            await jewelHandler.LoadAsyncLocal();
            jewelDataLoaded = true;
        }
    }

    public void SetMapPositionAndArea(Vector3 mapPosition, Vector2 oreAreaSize, Vector2 jewelAreaSize)
    {
        currentMapPosition = mapPosition;

        oreSpawner?.SetSpawnArea(currentMapPosition, oreAreaSize);
        jewelSpawner?.SetSpawnArea(currentMapPosition, jewelAreaSize);
    }

    public void OnMapChanged(Vector3 mapPosition, int mapIndex)
    {
        if (currentMapIndex == mapIndex)
        {
            // 같은 맵이면 기존 자원 활성화
            SetMapResourcesActive(mapIndex, true);
            return;
        }

        // 이전 맵 자원 비활성화
        if (currentMapIndex != -1)
            SetMapResourcesActive(currentMapIndex, false);

        currentMapIndex = mapIndex;

        SetMapPositionAndArea(mapPosition, defaultOreSpawnAreaSize, defaultJewelSpawnAreaSize);

        if (currentMapIndex == 0)
        {
            Debug.Log("기본맵이라 자원 스폰하지 않음");
            return;
        }

        if (mapResources.ContainsKey(currentMapIndex))
        {
            // 이미 스폰된 자원 활성화만
            SetMapResourcesActive(currentMapIndex, true);
        }
        else
        {
            // 새로 스폰하고 저장
            SpawnAllAndStore();
        }
    }

    private void SetMapResourcesActive(int mapIndex, bool active)
    {
        if (mapResources.TryGetValue(mapIndex, out var resources))
        {
            foreach (var obj in resources)
                obj.SetActive(active);
        }
    }

    private void SpawnAllAndStore()
    {
        if (!oreDataLoaded || !jewelDataLoaded) return;

        List<GameObject> spawnedObjects = new List<GameObject>();

        List<OreDatabase> oreList = oreHandler.GetAllItems();
        List<JewelDatabase> jewelList = jewelHandler.GetAllItems();

        if (oreSpawner != null && oreList != null)
        {
            oreSpawner.SpawnResources(oreList, currentStage);
            spawnedObjects.AddRange(GetChildrenGameObjects(oreSpawner.transform));
        }

        if (jewelSpawner != null && jewelList != null)
        {
            jewelSpawner.SpawnResources(jewelList, currentStage);
            spawnedObjects.AddRange(GetChildrenGameObjects(jewelSpawner.transform));
        }

        mapResources[currentMapIndex] = spawnedObjects;
    }

    private List<GameObject> GetChildrenGameObjects(Transform parent)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            list.Add(parent.GetChild(i).gameObject);
        }
        return list;
    }

    public void ChangeStage(int newStage)
    {
        currentStage = newStage;

        // 스테이지 변경 시 모든 자원 삭제 후 다시 스폰
        ClearAll();
        mapResources.Clear();

        SpawnAll();
    }

    public void SpawnAll()
    {
        if (!oreDataLoaded || !jewelDataLoaded) return;

        if (currentMapIndex == 0)
        {
            Debug.Log("기본맵이라 자원 스폰하지 않음");
            ClearAll();
            return;
        }

        List<OreDatabase> oreList = oreHandler.GetAllItems();
        List<JewelDatabase> jewelList = jewelHandler.GetAllItems();

        if (oreSpawner != null && oreList != null)
        {
            oreSpawner.SpawnResources(oreList, currentStage);
        }

        if (jewelSpawner != null && jewelList != null)
        {
            jewelSpawner.SpawnResources(jewelList, currentStage);
        }
    }

    private void ClearAll()
    {
        ClearChildren(oreSpawner?.transform);
        ClearChildren(jewelSpawner?.transform);
        mapResources.Clear();
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
}