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

    // (현재맵, 포탈방향) → 이동할 맵 인덱스
    private Dictionary<(int fromMapIndex, MiningHandler.PortalDirection dir), int> portalMapLinks
        = new Dictionary<(int, MiningHandler.PortalDirection), int>();


    private Vector2[] smallSpawnAreas = new Vector2[]
    {
        new Vector2(60f, 30f),
        new Vector2(60f, 30f)
    };

    private Vector2[] largeSpawnAreas = new Vector2[]
    {
        new Vector2(80f, 50f),
        new Vector2(80f, 50f)
    };

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

        int current = CurrentMapIndex;

        // 이미 연결된 맵이 있다면 해당 맵으로 이동
        if (portalMapLinks.TryGetValue((current, dir), out int linkedMapIndex))
        {
            MoveToMap(linkedMapIndex, true);
        }
        else
        {
            // 새로운 맵 인덱스 할당
            int newMapIndex = nextMapIndex++;
            MoveToMap(newMapIndex, true);

            // 현재 맵의 해당 방향 → 새 맵
            portalMapLinks[(current, dir)] = newMapIndex;

            // 새 맵의 반대 방향 → 현재 맵
            var oppositeDir = GetOppositeDirection(dir);
            portalMapLinks[(newMapIndex, oppositeDir)] = current;
        }
    }

    public void MoveToRandomMap()
    {
        MoveToMap(nextMapIndex, false);
        nextMapIndex++;
    }

    public void MoveToPreviousMap()
    {
        if (mapHistory.Count == 0) return;

        int prev = mapHistory.Pop();

        if (LastEnteredPortalDirection.HasValue)
            LastEnteredPortalDirection = GetOppositeDirection(LastEnteredPortalDirection.Value);

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

        var spawnManager = FindObjectOfType<SpawnManager>();

        if (CurrentMapIndex != 0)
        {
            spawnManager?.SetMapResourcesActive(CurrentMapIndex, false);
        }

        // 현재 맵 비활성화
        if (CurrentMapIndex == 0)
            baseMap.SetActive(false);
        else if (mapInstances.TryGetValue(CurrentMapIndex, out var currentChunk))
            currentChunk.SetActive(false);

        // 타겟 맵 생성 혹은 존재 여부 확인
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

        // 새 맵 활성화 및 플레이어 위치 조정
        if (targetIndex == 0)
        {
            baseMap.SetActive(true);
            Vector3 spawnPos = GetSpawnPositionByEnteredPortal(baseMap, LastEnteredPortalDirection);
            player.position = spawnPos;

            // 기본 맵은 스폰 안 함
            spawnManager?.OnMapChanged(baseMap.transform.position, 0, smallSpawnAreas);
        }
        else if (mapInstances.TryGetValue(targetIndex, out var nextMap))
        {
            nextMap.SetActive(true);

            Vector3 spawnPos = GetSpawnPositionByEnteredPortal(nextMap, LastEnteredPortalDirection);
            player.position = spawnPos;

            Vector2[] spawnAreasToUse;

            string prefabName = nextMap.name.ToLower();

            if (prefabName.Contains("01") || prefabName.Contains("02") || prefabName.Contains("03"))
            {
                spawnAreasToUse = smallSpawnAreas;
            }
            else
            {
                spawnAreasToUse = largeSpawnAreas;
            }

            spawnManager?.SetMapPositionAndArea(nextMap.transform.position, spawnAreasToUse[0], spawnAreasToUse[1]);
            spawnManager?.OnMapChanged(nextMap.transform.position, targetIndex, spawnAreasToUse);
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