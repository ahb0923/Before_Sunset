using System.Collections.Generic;
using UnityEngine;

public class OreController : MonoBehaviour, IPoolable
{
    public OreDatabase _data { get; private set; }

    private int _currentHP;

    [System.Serializable]
    public class DropPrefabEntry
    {
        public int id;
        public GameObject prefab;
    }

    [Header("드롭 프리팹 매핑")]
    [SerializeField] private List<DropPrefabEntry> dropPrefabs = new();

    private Dictionary<int, GameObject> _dropPrefabDict;

    [SerializeField] private int _id;
    public int GetId() => _id;

    public void OnInstantiate()
    {
        _data = DataManager.Instance.OreData.GetById(_id);

        _dropPrefabDict = new Dictionary<int, GameObject>();
        foreach (var entry in dropPrefabs)
        {
            if (!_dropPrefabDict.ContainsKey(entry.id))
            {
                _dropPrefabDict.Add(entry.id, entry.prefab);
            }
        }
    }

    public void OnGetFromPool()
    {
        _currentHP = _data.hp;
    }

    public void OnReturnToPool()
    {
        //
    }

    public bool CanBeMined(int pickaxePower)
    {
        if (_data == null) return false;
        return pickaxePower >= _data.def;
    }

    public bool Mine(int damage)
    {
        _currentHP -= damage;

        if (_currentHP <= 0)
        {
            DropItem();
            PoolManager.Instance.ReturnToPool(_id, gameObject);
            return true;
        }

        return false;
    }

    private void DropItem()
    {
        int dropId = _data.dropMineralId;

        if (_dropPrefabDict != null && _dropPrefabDict.TryGetValue(dropId, out var prefab) && prefab != null)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"[OreController] 드롭 프리팹이 존재하지 않거나 null입니다. ID: {dropId}");
        }
    }
}
