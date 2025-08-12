using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawn : MonoBehaviour
{
    [SerializeField] private List<Bounds> _spawnStoneAreas;
    [SerializeField] private List<Bounds> _spawnCooperAreas;

    [SerializeField] private int _oreAmount;

    private void Start()
    {
        SpawnOres();
    }

    public void SpawnOres()
    {
        // 돌 소환
        List<Vector3> stonePositions = GetRandomPositions(_spawnStoneAreas, _oreAmount);
        foreach(var position in stonePositions)
        {
            PoolManager.Instance.GetFromPool(0, position, transform);
        }

        // 구리 소환
        List<Vector3> cooperPositions = GetRandomPositions(_spawnCooperAreas, _oreAmount);
        foreach(var position in cooperPositions)
        {
            PoolManager.Instance.GetFromPool(1, position, transform);
        }
    }

    private List<Vector3> GetRandomPositions(List<Bounds> areas, int amount)
    {
        List<Vector3> randomPositions = new List<Vector3>();

        List<Vector3> positions = new List<Vector3>();
        foreach(Bounds bound in areas)
        {
            for (int x = 0; x < (int)bound.size.x; x++)
            {
                for (int y = 0; y < (int)bound.size.y; y++)
                {
                    Vector3 pos = transform.position + bound.min + new Vector3(0.5f + x, 0.5f + y);
                    if(!positions.Contains(pos)) positions.Add(pos);
                }
            }
        }

        while(randomPositions.Count != amount)
        {
            int randIdx = Random.Range(0, positions.Count);
            randomPositions.Add(positions[randIdx]);
            positions.RemoveAt(randIdx);
        }

        return randomPositions;
    }

    private void OnDrawGizmos()
    {
        // 돌 소환 위치 시각화
        Gizmos.color = Color.green;
        foreach (var spawnArea in _spawnStoneAreas)
        {
            Gizmos.DrawWireCube(transform.position + spawnArea.center, spawnArea.size);
        }

        // 구리 소환 위치 시각화
        Gizmos.color = Color.yellow;
        foreach (var spawnArea in _spawnCooperAreas)
        {
            Gizmos.DrawWireCube(transform.position + spawnArea.center, spawnArea.size);
        }
    }
}