using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManager : MonoSingleton<MapManager>
{
    [Header("맵 관련 설정")]
    [SerializeField] private GameObject baseMap;
    [SerializeField] private string mapFolder = "Maps";
    [SerializeField] private string mapPrefix = "Map_";
    [SerializeField] private int prefabCount = 4; // Map_01 ~ Map_04
    [SerializeField] private Transform player;
    [SerializeField] private float mapSpacing = 100f;

    private List<GameObject> prefabPool = new List<GameObject>();           // 프리팹 캐시
    private Dictionary<int, GameObject> mapInstances = new Dictionary<int, GameObject>(); // 인스턴스 저장
    private Stack<int> mapHistory = new Stack<int>();
    private int currentMapIndex = 0; // 0: baseMap
    private Dictionary<int, int> mapPrefabIndexMap = new Dictionary<int, int>();
    private int nextMapIndex = 1; // baseMap은 0번이므로 다음은 1부터 시작


    protected override void Awake()
    {
        base.Awake();
        LoadAllPrefabs();
        baseMap.SetActive(true);
        MoveToMap(0, false);
    }

    private void LoadAllPrefabs()
    {
        for (int i = 1; i <= prefabCount; i++)
        {
            string path = $"{mapFolder}/{mapPrefix}{i:D2}";
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                prefabPool.Add(prefab);
            }
            else
            {
                Debug.LogWarning($"Map prefab not found at: {path}");
            }
        }
    }

    public void MoveToRandomMap()
    {
        MoveToMap(nextMapIndex);
        nextMapIndex++; // 다음 호출 시 새로운 맵 생성되도록 증가
    }

    public void MoveToPreviousMap()
    {
        if (mapHistory.Count == 0) return;
        int prev = mapHistory.Pop();
        MoveToMap(prev, false);
    }

    public void ReturnToHomeMap()
    {
        if (currentMapIndex == 0) return;
        mapHistory.Clear();
        MoveToMap(0, false);
    }

    private void MoveToMap(int targetIndex, bool addToHistory = true)
    {
        if (targetIndex == currentMapIndex) return;

        // 현재 맵 비활성화
        if (currentMapIndex == 0)
            baseMap.SetActive(false);
        else if (mapInstances.TryGetValue(currentMapIndex, out var currentChunk))
            currentChunk.SetActive(false);

        var spawnManager = FindObjectOfType<SpawnManager>();

        // 맵 인스턴스가 없으면 생성
        if (targetIndex > 0 && !mapInstances.ContainsKey(targetIndex))
        {
            if (!mapPrefabIndexMap.ContainsKey(targetIndex))
            {
                int randomPrefabIndex = Random.Range(0, prefabPool.Count);
                mapPrefabIndexMap[targetIndex] = randomPrefabIndex;
            }

            int prefabIndex = mapPrefabIndexMap[targetIndex];
            GameObject prefab = prefabPool[prefabIndex];
            GameObject instance = Instantiate(prefab);
            instance.SetActive(false);

            Vector3 position = baseMap.transform.position + new Vector3(mapSpacing * targetIndex, 0f, 0f);
            instance.transform.position = position;

            var vcam = instance.GetComponentInChildren<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = player;
                vcam.LookAt = player;
            }

            mapInstances[targetIndex] = instance;
        }

        // 맵 활성화 및 플레이어 이동
        if (targetIndex == 0)
        {
            baseMap.SetActive(true);
            player.position = baseMap.transform.position;
            spawnManager?.OnMapChanged(baseMap.transform.position, 0);
        }
        else if (mapInstances.TryGetValue(targetIndex, out var nextMap))
        {
            nextMap.SetActive(true);
            player.position = nextMap.transform.position;

            Vector2 oreArea = new Vector2(40f, 40f);
            Vector2 jewelArea = new Vector2(25f, 25f);
            spawnManager?.SetMapPositionAndArea(nextMap.transform.position, oreArea, jewelArea);
            spawnManager?.OnMapChanged(nextMap.transform.position, targetIndex);
        }

        if (addToHistory)
            mapHistory.Push(currentMapIndex);

        currentMapIndex = targetIndex;
    }
}