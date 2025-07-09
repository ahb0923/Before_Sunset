using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

    private bool _isSet;

    private Dictionary<int, GameObject> _prefabs;
    private Dictionary<int, Queue<GameObject>> _pools;

    /// <summary>
    /// 오브젝트 풀링 세팅
    /// </summary>
    public void InitPool()
    {
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
