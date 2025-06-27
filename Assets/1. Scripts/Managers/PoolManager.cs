using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnGetFromPool();
    void OnReturnToPool();
}

[System.Serializable]
public class ObjectPoolData
{
    public int id;
    public GameObject prefab;
    public int count;
}

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private List<ObjectPoolData> _objectPoolDatas = new List<ObjectPoolData>();

    // 일단 키 값을 ID로 받아오는데 enum으로 바꿔야 할 지 생각 중
    private Dictionary<int, GameObject> _prefabs;
    private Dictionary<int, Queue<GameObject>> _pools;

    protected override void Awake()
    {
        _prefabs = new Dictionary<int, GameObject>();
        _pools = new Dictionary<int, Queue<GameObject>>();

        foreach (var data in _objectPoolDatas)
        {
            if (!data.prefab.TryGetComponent<IPoolable>(out var poolable)) continue;

            _prefabs[data.id] = data.prefab;
            
            var queue = new Queue<GameObject>();
            _pools[data.id] = queue;

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
    public GameObject GetFromPool(int id)
    {
        if (!_pools.ContainsKey(id))
        {
            Debug.LogWarning($"[PoolManager] 가져오려는 타입이 등록되어 있지 않습니다.");
            return null;
        }

        Queue<GameObject> queue = _pools[id];
        GameObject obj;

        if(queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = Instantiate(_prefabs[id]);
            obj.SetActive(false);
        }

        obj.transform.SetParent(transform); // 요건 풀 매니저 밑으로 달아놓긴 했는데, 수정 가능
        obj.SetActive(true);

        obj.GetComponent<IPoolable>()?.OnGetFromPool();
        return obj;
    }

    /// <summary>
    /// 해당 타입의 오브젝트를 풀에 반환
    /// </summary>
    public void ReturnToPool(int id, GameObject obj)
    {
        if (!_pools.ContainsKey(id))
        {
            Debug.LogWarning($"[PoolManager] 반환하려는 타입이 등록되어 있지 않습니다.");
            Destroy(obj);
            return;
        }

        obj.GetComponent<IPoolable>()?.OnReturnToPool();
        obj.SetActive(false);
        _pools[id].Enqueue(obj);
    }
}
