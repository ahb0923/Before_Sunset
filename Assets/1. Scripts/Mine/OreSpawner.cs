using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OreSpawner : MonoBehaviour
{
    [Header("스폰 영역(직사각형)")]
    [SerializeField] private Vector2 spawnAreaCenter;
    [SerializeField] private Vector2 spawnAreaSize;

    [Header("스폰 개수")]
    [SerializeField] private int spawnCount = 20;

    [Header("광석 프리팹 리스트")]
    [SerializeField] private List<GameObject> orePrefabs;

    [Header("광석 간 중복 방지 반지름")]
    [SerializeField] private float overlapRadius = 1.0f;

    [Header("장애물 위 스폰 방지용 레이어 마스크")]
    [SerializeField] private LayerMask obstacleLayerMask;

    private Dictionary<int, GameObject> orePrefabCache;
    private List<OreData> spawnableOreList = new();
    private List<Vector3> placedOrePositions = new();

    private IEnumerator Start()
    {
        yield return DataManager.Instance.OreData.LoadAsyncLocal();
        SpawnOres(1);
    }

    private void Awake()
    {
        orePrefabCache = new Dictionary<int, GameObject>();

        foreach (var prefab in orePrefabs)
        {
            int id = ParseIdFromName(prefab.name);
            if (id >= 0)
                orePrefabCache[id] = prefab;
        }
    }

    private int ParseIdFromName(string name)
    {
        int underscoreIndex = name.LastIndexOf('_');
        if (underscoreIndex < 0 || underscoreIndex + 1 >= name.Length)
            return -1;

        string idStr = name.Substring(underscoreIndex + 1);
        if (int.TryParse(idStr, out int id))
            return id;

        return -1;
    }

    public void SpawnOres(int currentStage)
    {
        spawnableOreList.Clear();
        placedOrePositions.Clear();

        var allOres = DataManager.Instance.OreData.GetAllItems();

        foreach (var ore in allOres)
        {
            if (currentStage >= ore.spawnStage)
                spawnableOreList.Add(ore);
        }

        if (spawnableOreList.Count == 0)
            return;

        int placed = 0;
        int attempts = 0;
        int maxAttempts = spawnCount * 10;

        while (placed < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomPos = GetRandomPositionInArea();

            if (Physics2D.OverlapCircle(randomPos, 0.1f, obstacleLayerMask) != null) continue;

            if (IsTooCloseToOtherOres(randomPos)) continue;

            OreData selectedOre = GetRandomOreByProbability();
            if (selectedOre != null && orePrefabCache.TryGetValue(selectedOre.id, out var prefab))
            {
                GameObject oreObj = Instantiate(prefab, randomPos, Quaternion.identity);
                var oreController = oreObj.GetComponent<OreController>();
                if (oreController != null)
                {
                    oreController.Initialize(selectedOre);
                    placedOrePositions.Add(randomPos);
                    placed++;
                }
            }
        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        float x = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2f, spawnAreaCenter.x + spawnAreaSize.x / 2f);
        float y = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2f, spawnAreaCenter.y + spawnAreaSize.y / 2f);
        return new Vector3(x, y, 0f);
    }

    private bool IsTooCloseToOtherOres(Vector3 pos)
    {
        foreach (var otherPos in placedOrePositions)
        {
            if (Vector3.Distance(pos, otherPos) < overlapRadius)
                return true;
        }
        return false;
    }

    private OreData GetRandomOreByProbability()
    {
        int totalWeight = spawnableOreList.Sum(o => o.spawnProbability);
        if (totalWeight <= 0)
            return null;

        int rand = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var ore in spawnableOreList)
        {
            cumulative += ore.spawnProbability;
            if (rand < cumulative)
                return ore;
        }

        return null;
    }
}
