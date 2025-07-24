using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private string[] _zoneLayerNames = { "Zone1", "Zone2", "Zone3" };
    [SerializeField] private float _tileSize = 1.0f;

    private OreSpawner _oreSpawner;
    private JewelSpawner _jewelSpawner;
    private OreDataHandler _oreHandler;
    private JewelDataHandler _jewelHandler;

    private List<GameObject> _spawnedObjects = new();
    private HashSet<Vector3> _usedTilePositions = new();

    private void Start()
    {
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
                        SpawnOres(bounds, 2, 2, 3, 5, 7);
                        break;
                    case 1:
                        SpawnOres(bounds, 5, 10, 20, 20, 5);
                        break;
                    case 2:
                        SpawnOres(bounds, 55, 35, 20, 5, 0);
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
                    _spawnedObjects.Add(_jewelSpawner.SpawnSingle(jewel, pos.Value));
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
                    _spawnedObjects.Add(_oreSpawner.SpawnSingle(ore, pos.Value));
                }
                else
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 영역 내에서 타일 중앙 위치 중복x
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
                Vector3 tileCenter = new Vector3(x, y, 0f);
                if (bounds.Contains(tileCenter))
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
}