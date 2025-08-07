using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    int GetId();
    void OnInstantiate();
    void OnGetFromPool();
    void OnReturnToPool();
}



[System.Serializable]
public class ObjectPoolData
{
    public GameObject prefab;
    public int count;
}

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private List<ObjectPoolData> _objectPoolDatas = new List<ObjectPoolData>();

    // 추후에 정리해서 데이터로 만들것.
    [SerializeField] private GameObject _electircline;
    [SerializeField] private GameObject _aoeEffect;
    [SerializeField] private GameObject _buildGuage;

    private bool _isSet;

    // 임시방편 코드 추후 데이터 제작 논의
    private Dictionary<int, GameObject> _prefabs;
    private Dictionary<int, Queue<GameObject>> _pools;

    protected override void Awake()
    {
        base.Awake();
        if(Instance != null)
            DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// 오브젝트 풀링 세팅
    /// </summary>
    public void InitPool()
    {
        SettingPrefab();

        _prefabs = new Dictionary<int, GameObject>();
        _pools = new Dictionary<int, Queue<GameObject>>();

        foreach (var data in _objectPoolDatas)
        {
            if (!data.prefab.TryGetComponent<IPoolable>(out var poolable)) continue;

            _prefabs[poolable.GetId()] = data.prefab;
            //Debug.Log($"등록할 ID : { poolable.GetId()}");
            
            var queue = new Queue<GameObject>();
            _pools[poolable.GetId()] = queue;

            for (int i = 0; i < data.count; i++)
            {
                GameObject obj = Instantiate(data.prefab, transform);
                obj.GetComponent<IPoolable>().OnInstantiate();
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
        }

        _isSet = true;
    }
    public void SettingPrefab()
    {
        _objectPoolDatas.Clear();

        // 이부분 일단 하드코딩입니다.////////////////////
        _objectPoolDatas.Add(new ObjectPoolData
        {
            prefab = _electircline,
            count = 10
        });
        _objectPoolDatas.Add(new ObjectPoolData
        {
            prefab = _aoeEffect,
            count = 10
        });
        
        _objectPoolDatas.Add(new ObjectPoolData
        {
            prefab = _buildGuage,
            count = 10
        });
        // 여기까지 ///////////////////////////////////////

        foreach (var data in DataManager.Instance.MonsterData.GetAllItems())
        {
            var prefab = DataManager.Instance.MonsterData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 20
                });
            }
        }
        foreach (var data in DataManager.Instance.TowerData.GetAllItems())
        {
            var prefab = DataManager.Instance.TowerData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 1
                });
            }
        }
        foreach (var data in DataManager.Instance.ProjectileData.GetAllItems())
        {
            var prefab = DataManager.Instance.ProjectileData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 10
                });
            }
        }
        foreach (var data in DataManager.Instance.SmelterData.GetAllItems())
        {
            var prefab = DataManager.Instance.SmelterData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 10
                });
            }
        }
        foreach (var data in DataManager.Instance.OreData.GetAllItems())
        {
            var prefab = DataManager.Instance.OreData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 10
                });
            }
        }
        foreach (var data in DataManager.Instance.JewelData.GetAllItems())
        {
            var prefab = DataManager.Instance.JewelData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 10
                });
            }
        }
        foreach (var data in DataManager.Instance.MineralData.GetAllItems())
        {
            var prefab = DataManager.Instance.MineralData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 10
                });
            }
        }
        foreach (var data in DataManager.Instance.MapData.GetAllItems())
        {
            var prefab = DataManager.Instance.MapData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 1
                });
            }
        }
        foreach (var data in DataManager.Instance.DebuffData.GetAllItems())
        {
            var prefab = DataManager.Instance.DebuffData.GetPrefabById(data.id);
            if (prefab != null)
            {
                _objectPoolDatas.Add(new ObjectPoolData
                {
                    prefab = prefab,
                    count = 10
                });
            }
        }
    }
    /// <summary>
    /// 해당 타입의 오브젝트를 풀에서 가져와서 반환, 없으면 새로 생성해서 반환
    /// </summary>
    public GameObject GetFromPool(int id, Vector3 spawnPos, Transform parent = null)
    {
        if (!_isSet)
        {
            InitPool();
        }

        if (!_pools.ContainsKey(id))
        {
            Debug.LogWarning($"[PoolManager] 가져오려는 ID가 등록되어 있지 않습니다. : {id}");
            return null;
        }

        GameObject obj;
        if(_pools[id].Count > 0)
        {
            obj = _pools[id].Dequeue();
        }
        else
        {
            obj = Instantiate(_prefabs[id]);
            obj.GetComponent<IPoolable>().OnInstantiate();
        }

        obj.transform.position = spawnPos;
        obj.transform.SetParent(parent == null ? transform : parent);

        obj.SetActive(true);
        obj.GetComponent<IPoolable>().OnGetFromPool();
        return obj;
    }

    /// <summary>
    /// 해당 타입의 오브젝트를 풀에 반환
    /// </summary>
    public void ReturnToPool(int id, GameObject obj)
    {
        if (!_isSet)
        {
            InitPool();
        }

        if (!_pools.ContainsKey(id))
        {
            Debug.LogWarning($"[PoolManager] 반환하려는 ID가 등록되어 있지 않습니다. : {id}");
            Destroy(obj);
            return;
        }

        obj.transform.SetParent(transform);

        obj.GetComponent<IPoolable>()?.OnReturnToPool();
        obj.SetActive(false);
        _pools[id].Enqueue(obj);
    }

}
