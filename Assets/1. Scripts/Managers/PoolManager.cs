using System.Collections.Generic;
using UnityEngine;

public enum POOL_TYPE
{
    Monster,
    Projectile,
    Mineral,
    // 필요 시 풀링될 오브젝트 타입 추가
}

[System.Serializable]
public class ObjectPoolData
{
    public POOL_TYPE type;
    public GameObject prefab;
    public int count;
}

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private List<ObjectPoolData> _objectPoolDatas = new List<ObjectPoolData>();
    private Dictionary<POOL_TYPE, GameObject> _prefabs;

    private Dictionary<POOL_TYPE, Queue<GameObject>> _pools;

    protected override void Awake()
    {
        base.Awake();

        _prefabs = new Dictionary<POOL_TYPE, GameObject>();
        _pools = new Dictionary<POOL_TYPE, Queue<GameObject>>();

        foreach (var data in _objectPoolDatas)
        {
            _prefabs[data.type] = data.prefab;

            var queue = new Queue<GameObject>();
            _pools[data.type] = queue;

            for (int i = 0; i < data.count; i++)
            {
                GameObject obj = Instantiate(data.prefab, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
        }
    }

    /// <summary>
    /// 해당 타입의 오브젝트를 풀에서 가져와서 반환, 없으면 새로 생성해서 반환<br/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetFromPool(POOL_TYPE type)
    {
        if (!_pools.ContainsKey(type))
        {
            Debug.LogWarning($"[PoolManager] 가져오려는 타입이 등록되어 있지 않음 : {type}");
            return null;
        }

        Queue<GameObject> queue = _pools[type];
        GameObject obj;

        if(queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = Instantiate(_prefabs[type]);
            obj.SetActive(false);
        }

        obj.transform.SetParent(transform);
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 해당 타입의 오브젝트를 풀에 반환
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    public void ReturnToPool(POOL_TYPE type, GameObject obj)
    {
        if (!_pools.ContainsKey(type))
        {
            Debug.LogWarning($"[PoolManager] 반환하려는 타입이 등록되어 있지 않음 : {type}");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        _pools[type].Enqueue(obj);
    }
}
