using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private int _size;
    private float _halfSize => _size * 0.5f;

    public List<Vector3> GetNearestNodePositionsFromCore()
    {
        Vector3 bottomLeftPos = transform.position - new Vector3(_halfSize + MonsterSpawner.NODE_HALF_SIZE, _halfSize + MonsterSpawner.NODE_HALF_SIZE);

        List<Vector3> positions = new List<Vector3>();
        for (int x = 0; x < _size + 2; x++) 
        {
            for (int y = 0; y < _size + 2; y++) 
            {
                if(x == 0 || y == 0 || x == _size + 1 || y == _size + 1)
                {
                    Vector3 pos = bottomLeftPos + new Vector3(x * MonsterSpawner.NODE_SIZE, y * MonsterSpawner.NODE_SIZE, 0);
                    positions.Add(pos);
                }
            }
        }

        return positions;
    }
}
