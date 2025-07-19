using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManager : MonoSingleton<MapManager>
{
    public int CurrentMapIndex { get; private set; } = 0;

    private GameObject _baseMap;
    private Transform _player;

    [SerializeField] private string _mapFolder = "Maps";
    [SerializeField] private string _mapPrefix = "Map_";
    [SerializeField] private int _prefabCount = 4;
    [SerializeField] private float _mapSpacing = 100f;

    private int _nextMapIndex = 1;
    private List<GameObject> _prefabPool = new List<GameObject>();
    private Dictionary<int, GameObject> _mapInstances = new Dictionary<int, GameObject>();
    private Stack<int> _mapHistory = new Stack<int>();
    private Dictionary<int, int> _mapPrefabIndexMap = new Dictionary<int, int>();

    private Dictionary<(int, Portal.PortalDirection), int> _portalMapLinks = new();

    private SpawnManager _spawnManager;

    protected override void Awake()
    {
        base.Awake();

        // BaseMap과 Player를 이름으로 찾아서 자동 할당
        if (_baseMap == null)
        {
            var baseMapGO = GameObject.Find("BaseMap");
            if (baseMapGO != null)
                _baseMap = baseMapGO;
            else
                Debug.LogError("BaseMap 오브젝트를 찾을 수 없습니다. 이름이 'BaseMap'인지 확인하세요.");
        }

        if (_player == null)
        {
            var playerGO = GameObject.Find("Player");
            if (playerGO != null)
                _player = playerGO.transform;
            else
                Debug.LogError("Player 오브젝트를 찾을 수 없습니다. 이름이 'Player'인지 확인하세요.");
        }

        LoadAllPrefabs();
        _baseMap.SetActive(true);
        _spawnManager = FindObjectOfType<SpawnManager>();
        MoveToMap(0, false);
    }


    private void LoadAllPrefabs()
    {
        _prefabPool.Clear();

        for (int i = 1; i <= _prefabCount; i++)
        {
            string path = $"{_mapFolder}/{_mapPrefix}{i:D2}";
            var prefab = Resources.Load<GameObject>(path);
            if (prefab != null) _prefabPool.Add(prefab);
            else Debug.LogWarning($"Map prefab not found: {path}");
        }
    }

    public void MoveToMapByDirection(Portal.PortalDirection dir)
    {
        int current = CurrentMapIndex;

        if (_portalMapLinks.TryGetValue((current, dir), out int linkedMapIndex))
        {
            MoveToMap(linkedMapIndex, true);
        }
        else
        {
            int newMapIndex = _nextMapIndex++;
            MoveToMap(newMapIndex, true);

            _portalMapLinks[(current, dir)] = newMapIndex;

            var oppositeDir = PortalManager.Instance.GetOppositeDirection(dir);
            _portalMapLinks[(newMapIndex, oppositeDir)] = current;
        }
    }

    public void MoveToRandomMap()
    {
        MoveToMap(_nextMapIndex++, false);
    }

    public void MoveToPreviousMap()
    {
        if (_mapHistory.Count == 0) return;

        int prev = _mapHistory.Pop();

        if (PortalManager.Instance.LastEnteredPortalDirection.HasValue)
            PortalManager.Instance.LastEnteredPortalDirection = PortalManager.Instance.GetOppositeDirection(PortalManager.Instance.LastEnteredPortalDirection.Value);

        MoveToMap(prev, false);
    }

    public void ReturnToHomeMap()
    {
        if (CurrentMapIndex == 0) return;
        _mapHistory.Clear();
        MoveToMap(0, false);
    }

    private void MoveToMap(int targetIndex, bool addToHistory = true)
    {
        if (targetIndex == CurrentMapIndex) return;

        _spawnManager?.SetMapResourcesActive(CurrentMapIndex, false);

        if (CurrentMapIndex == 0)
            _baseMap.SetActive(false);
        else if (_mapInstances.TryGetValue(CurrentMapIndex, out var currentChunk))
            currentChunk.SetActive(false);

        if (targetIndex > 0 && !_mapInstances.ContainsKey(targetIndex))
        {
            if (!_mapPrefabIndexMap.ContainsKey(targetIndex))
            {
                int prefabIndex = Random.Range(0, _prefabPool.Count);
                _mapPrefabIndexMap[targetIndex] = prefabIndex;
            }

            int pIndex = _mapPrefabIndexMap[targetIndex];
            GameObject prefab = _prefabPool[pIndex];
            GameObject instance = Instantiate(prefab);
            instance.SetActive(false);
            instance.transform.position = _baseMap.transform.position + new Vector3(_mapSpacing * targetIndex, 0, 0);

            var vcam = instance.GetComponentInChildren<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = _player;
                vcam.LookAt = _player;
            }

            _mapInstances[targetIndex] = instance;
        }

        if (targetIndex == 0)
        {
            _baseMap.SetActive(true);
            Vector3 spawnPos = GetSpawnPositionByEnteredPortal(_baseMap, PortalManager.Instance.LastEnteredPortalDirection);
            _player.position = spawnPos;

            Vector2[] smallSpawnAreas = new Vector2[] { new Vector2(60f, 32f), new Vector2(60f, 32f) };

            _spawnManager?.OnMapChanged(_baseMap.transform.position, 0, smallSpawnAreas);
        }
        else if (_mapInstances.TryGetValue(targetIndex, out var nextMap))
        {
            nextMap.SetActive(true);
            Vector3 spawnPos = GetSpawnPositionByEnteredPortal(nextMap, PortalManager.Instance.LastEnteredPortalDirection);
            _player.position = spawnPos;

            Vector2[] smallSpawnAreas = new Vector2[] { new Vector2(60f, 32f), new Vector2(60f, 32f) };
            Vector2[] largeSpawnAreas = new Vector2[] { new Vector2(85f, 50f), new Vector2(85f, 50f) };

            string prefabName = nextMap.name.ToLower();

            Vector2[] spawnAreasToUse = (prefabName.Contains("01") || prefabName.Contains("02") || prefabName.Contains("03"))
                ? smallSpawnAreas
                : largeSpawnAreas;

            _spawnManager?.SetMapPositionAndArea(nextMap.transform.position, spawnAreasToUse[0], spawnAreasToUse[1]);
            _spawnManager?.OnMapChanged(nextMap.transform.position, targetIndex, spawnAreasToUse);
        }

        if (addToHistory)
            _mapHistory.Push(CurrentMapIndex);

        CurrentMapIndex = targetIndex;
    }

    private Vector3 GetSpawnPositionByEnteredPortal(GameObject mapInstance, Portal.PortalDirection? enteredDir)
    {
        if (enteredDir == null)
            return mapInstance.transform.position;

        var exitDir = PortalManager.Instance.GetOppositeDirection(enteredDir.Value);

        var offsetByDirection = new Dictionary<Portal.PortalDirection, Vector3>()
        {
            { Portal.PortalDirection.North, new Vector3(0, -2f, 0) },
            { Portal.PortalDirection.South, new Vector3(0, 2f, 0) },
            { Portal.PortalDirection.East,  new Vector3(-2f, 0, 0) },
            { Portal.PortalDirection.West,  new Vector3(2f, 0, 0) }
        };

        var portals = mapInstance.GetComponentsInChildren<Portal>();

        foreach (var portal in portals)
        {
            if (portal.CurrentPortalDirection == exitDir)
            {
                Vector3 offset = offsetByDirection.TryGetValue(exitDir, out var v) ? v : Vector3.zero;
                return portal.transform.position + offset;
            }
        }

        return mapInstance.transform.position;
    }
}