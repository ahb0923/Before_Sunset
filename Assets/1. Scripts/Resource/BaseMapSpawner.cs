using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;


public class BaseMapSpawner : MonoBehaviour, ISaveable
{
    [SerializeField] private string[] _zoneLayerNames = { "Zone1", "Zone2", "Zone3" };
    [SerializeField] private float _tileSize = 1.0f;

    private OreSpawner _oreSpawner;
    private JewelSpawner _jewelSpawner;
    private OreDataHandler _oreHandler;
    private JewelDataHandler _jewelHandler;

    private List<GameObject> _spawnedObjects = new();
    private HashSet<Vector3> _usedTilePositions = new();

    private Transform _baseResourceParent;
    private int _wallLayer;

    private void Start()
    {
        _wallLayer = LayerMask.NameToLayer("Wall");
        StartCoroutine(WaitForInit());
    }

    private IEnumerator WaitForInit()
    {
        while (!DataManagerReady())
            yield return null;

        _oreSpawner = Helper_Component.FindChildComponent<OreSpawner>(transform.parent, "OreSpawner");
        _jewelSpawner = Helper_Component.FindChildComponent<JewelSpawner>(transform.parent, "JewelSpawner");

        _oreHandler = DataManager.Instance.OreData;
        _jewelHandler = DataManager.Instance.JewelData;

        GameObject baseMapGO = GameObject.Find("BaseMap");
        if (baseMapGO == null)
        {
            Debug.LogError("BaseMap 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        _baseResourceParent = baseMapGO.transform.Find("BaseMapResource");
        if (_baseResourceParent == null)
        {
            Debug.LogError("BaseMap 안에서 BaseMapResource 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        SpawnAllZones();
    }

    private bool DataManagerReady()
    {
        var task = DataManager.Instance.InitCheck();
        return task.IsCompleted && DataManager.Instance.OreData != null && DataManager.Instance.JewelData != null;
    }

    private void SpawnAllZones()
    {
        _usedTilePositions.Clear();
        _spawnedObjects.Clear();

        for (int group = 0; group < _zoneLayerNames.Length; group++)
        {
            string layerName = _zoneLayerNames[group];
            int layer = LayerMask.NameToLayer(layerName);
            if (layer < 0)
            {
                continue;
            }

            var colliders = FindObjectsOfType<Collider2D>()
                .Where(c => c.gameObject.layer == layer)
                .ToArray();


            foreach (var collider in colliders)
            {
                Bounds bounds = collider.bounds;

                switch (group)
                {
                    case 0:
                        SpawnJewels(bounds, 2);
                        SpawnOres(bounds, 0, 0, 0, 8, 10);
                        break;
                    case 1:
                        SpawnOres(bounds, 3, 5, 25, 20, 5);
                        break;
                    case 2:
                        SpawnOres(bounds, 59, 40, 0, 0, 0);
                        break;
                }
            }
        }
    }

    private void SpawnJewels(Bounds bounds, int countPerType)
    {
        var jewelList = _jewelHandler.GetAllItems();
        foreach (var jewel in jewelList)
        {
            for (int i = 0; i < countPerType; i++)
            {
                Vector3? pos = GetRandomTileCenterPosition(bounds);
                if (pos.HasValue)
                {
                    var obj = _jewelSpawner.SpawnSingle(jewel, pos.Value);
                    obj.transform.SetParent(_baseResourceParent);
                    _spawnedObjects.Add(obj);
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void SpawnOres(Bounds bounds, int stone, int copper, int iron, int silver, int gold)
    {
        var oreList = _oreHandler.GetAllItems();
        foreach (var ore in oreList)
        {
            int count = ore.itemName switch
            {
                "돌광석" => stone,
                "구리광석" => copper,
                "철광석" => iron,
                "은광석" => silver,
                "금광석" => gold,
                _ => 0
            };

            for (int i = 0; i < count; i++)
            {
                Vector3? pos = GetRandomTileCenterPosition(bounds);
                if (pos.HasValue)
                {
                    var obj = _oreSpawner.SpawnSingle(ore, pos.Value);
                    obj.transform.SetParent(_baseResourceParent);
                    _spawnedObjects.Add(obj);
                }
                else
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 영역 내에서 타일 중앙 위치 중복x, Wall 레이어 제외
    /// </summary>
    private Vector3? GetRandomTileCenterPosition(Bounds bounds)
    {
        List<Vector3> potentialTileCenters = new List<Vector3>();

        float startX = Mathf.Floor(bounds.min.x / _tileSize) * _tileSize + _tileSize / 2f;
        float startY = Mathf.Floor(bounds.min.y / _tileSize) * _tileSize + _tileSize / 2f;

        for (float x = startX; x < bounds.max.x; x += _tileSize)
        {
            for (float y = startY; y < bounds.max.y; y += _tileSize)
            {
                Vector3 tileCenter = new Vector3(x + 0.5f, y + 0.5f, 0f);
                if (bounds.Contains(tileCenter) && !IsPositionOnWallLayer(tileCenter))
                {
                    potentialTileCenters.Add(tileCenter);
                }
            }
        }

        potentialTileCenters = potentialTileCenters.OrderBy(a => Random.value).ToList();

        foreach (Vector3 candidate in potentialTileCenters)
        {
            if (!_usedTilePositions.Contains(candidate))
            {
                _usedTilePositions.Add(candidate);
                return candidate;
            }
        }

        return null;
    }

    /// <summary>
    /// 기지 광석 정보 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        foreach(var obj in _spawnedObjects)
        {
            if(obj.TryGetComponent<IResourceStateSavable>(out var resource))
            {
                ResourceState resourceState = resource.SaveState();
                if (!resourceState.IsMined)
                    data.baseResources.Add(new ResourceSaveData(resourceState.Id, resourceState.Position, resourceState.HP));
            }
        }
    }

    /// <summary>
    /// 기지 광석 정보 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        foreach (var obj in _spawnedObjects)
        {
            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                PoolManager.Instance.ReturnToPool(poolable.GetId(), obj);
            }
        }

        _spawnedObjects.Clear();

        foreach (var resourceData in data.baseResources)
        {
            ResourceState resource = new ResourceState()
            {
                Id = resourceData.resourceId,
                Position = resourceData.position,
                HP = resourceData.curHp,
                IsMined = false
            };

            GameObject spawned = PoolManager.Instance.GetFromPool(resource.Id, resource.Position, _baseResourceParent);
            if (spawned.TryGetComponent<IResourceStateSavable>(out var stateable))
            {
                stateable.LoadState(resource);
            }

            _spawnedObjects.Add(spawned);
        }
    }
    
    /// 해당 위치가 Wall 레이어의 콜라이더와 겹치는지 확인
    /// </summary>
    private bool IsPositionOnWallLayer(Vector3 position)
    {
        if (_wallLayer < 0) return false;

        Collider2D wallCollider = Physics2D.OverlapPoint(position, 1 << _wallLayer);
        return wallCollider != null;
    }
}