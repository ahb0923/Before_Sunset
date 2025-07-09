using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManager : MonoSingleton<MapManager>
{
    public MiningHandler.PortalDirection? LastEnteredPortalDirection { get; private set; }
    public int CurrentMapIndex { get; private set; } = 0;

    [SerializeField] private GameObject baseMap;
    [SerializeField] private string mapFolder = "Maps";
    [SerializeField] private string mapPrefix = "Map_";
    [SerializeField] private int prefabCount = 4;
    [SerializeField] private Transform player;
    [SerializeField] private float mapSpacing = 100f;

    private int nextMapIndex = 1;
    private List<GameObject> prefabPool = new List<GameObject>();
    private Dictionary<int, GameObject> mapInstances = new Dictionary<int, GameObject>();
    private Stack<int> mapHistory = new Stack<int>();
    private Dictionary<int, int> mapPrefabIndexMap = new Dictionary<int, int>();

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
            var prefab = Resources.Load<GameObject>(path);
            if (prefab != null) prefabPool.Add(prefab);
            else Debug.LogWarning($"Map prefab not found: {path}");
        }
    }

    public void MoveToMapByDirection(MiningHandler.PortalDirection dir)
    {
        LastEnteredPortalDirection = dir;
        MoveToRandomMap();
    }

    public void MoveToRandomMap()
    {
        MoveToMap(nextMapIndex);
        nextMapIndex++;
    }

    public void MoveToPreviousMap()
    {
        if (mapHistory.Count == 0) return;

        int prev = mapHistory.Pop();
        MoveToMap(prev, false);
    }

    public void ReturnToHomeMap()
    {
        if (CurrentMapIndex == 0) return;
        mapHistory.Clear();
        MoveToMap(0, false);
    }

    private void MoveToMap(int targetIndex, bool addToHistory = true)
    {
        if (targetIndex == CurrentMapIndex) return;

        // 현재 맵 비활성화
        if (CurrentMapIndex == 0)
            baseMap.SetActive(false);
        else if (mapInstances.TryGetValue(CurrentMapIndex, out var currentChunk))
            currentChunk.SetActive(false);

        var spawnManager = FindObjectOfType<SpawnManager>();

        if (targetIndex > 0 && !mapInstances.ContainsKey(targetIndex))
        {
            if (!mapPrefabIndexMap.ContainsKey(targetIndex))
            {
                int prefabIndex = Random.Range(0, prefabPool.Count);
                mapPrefabIndexMap[targetIndex] = prefabIndex;
            }

            int pIndex = mapPrefabIndexMap[targetIndex];
            GameObject prefab = prefabPool[pIndex];
            GameObject instance = Instantiate(prefab);
            instance.SetActive(false);

            instance.transform.position = baseMap.transform.position + new Vector3(mapSpacing * targetIndex, 0, 0);

            var vcam = instance.GetComponentInChildren<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = player;
                vcam.LookAt = player;
            }

            mapInstances[targetIndex] = instance;
        }

        if (targetIndex == 0)
        {
            baseMap.SetActive(true);
            player.position = baseMap.transform.position;
            spawnManager?.OnMapChanged(baseMap.transform.position, 0);
        }
        else if (mapInstances.TryGetValue(targetIndex, out var nextMap))
        {
            nextMap.SetActive(true);

            // 플레이어 위치를 입장한 포탈 방향의 반대 방향 포탈 위치로 세팅
            Vector3 spawnPos = GetSpawnPositionByEnteredPortal(nextMap, LastEnteredPortalDirection);
            player.position = spawnPos;

            Vector2 oreArea = new Vector2(40f, 40f);
            Vector2 jewelArea = new Vector2(25f, 25f);
            spawnManager?.SetMapPositionAndArea(nextMap.transform.position, oreArea, jewelArea);
            spawnManager?.OnMapChanged(nextMap.transform.position, targetIndex);
        }

        if (addToHistory)
            mapHistory.Push(CurrentMapIndex);

        CurrentMapIndex = targetIndex;
    }

    private Vector3 GetSpawnPositionByEnteredPortal(GameObject mapInstance, MiningHandler.PortalDirection? enteredDir)
    {
        if (enteredDir == null)
            return mapInstance.transform.position;

        var exitDir = GetOppositeDirection(enteredDir.Value);

        var portals = mapInstance.GetComponentsInChildren<MiningHandler>();

        foreach (var portal in portals)
        {
            if (portal.CurrentPortalDirection == exitDir)
            {
                Vector3 pos = portal.transform.position;
                switch (exitDir)
                {
                    case MiningHandler.PortalDirection.North:
                        pos.y -= 2f;
                        break;
                    case MiningHandler.PortalDirection.South:
                        pos.y += 2f;
                        break;
                    case MiningHandler.PortalDirection.East:
                        pos.x -= 2f;
                        break;
                    case MiningHandler.PortalDirection.West:
                        pos.x += 2f;
                        break;
                }
                return pos;
            }
        }

        return mapInstance.transform.position;
    }


    private MiningHandler.PortalDirection GetOppositeDirection(MiningHandler.PortalDirection dir)
    {
        return dir switch
        {
            MiningHandler.PortalDirection.North => MiningHandler.PortalDirection.South,
            MiningHandler.PortalDirection.South => MiningHandler.PortalDirection.North,
            MiningHandler.PortalDirection.East => MiningHandler.PortalDirection.West,
            MiningHandler.PortalDirection.West => MiningHandler.PortalDirection.East,
            _ => dir,
        };
    }
}
