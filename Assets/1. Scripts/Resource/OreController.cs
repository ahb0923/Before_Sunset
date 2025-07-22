using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class OreController : MonoBehaviour, IPoolable, IInteractable, IResourceStateSavable
{
    public OreDatabase _data { get; private set; }

    private BasePlayer _player;

    private Collider2D _collider;

    private int _currentHP;

    [SerializeField] private int _id;
    public int GetId() => _id;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void Init(BasePlayer basePlayer)
    {
        _player = basePlayer;
    }

    public void OnInstantiate()
    {
        _data = DataManager.Instance.OreData.GetById(_id);
    }

    public void OnGetFromPool()
    {
        _currentHP = _data.hp;
    }

    public void OnReturnToPool()
    {
        //
    }

    public ResourceState SaveState()
    {
        return new ResourceState
        {
            Id = _id,
            Position = transform.position,
            HP = _currentHP
        };
    }

    public void LoadState(ResourceState state)
    {
        _currentHP = state.HP;
        transform.position = state.Position;
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

        GameObject dropObj = PoolManager.Instance.GetFromPool(dropId, transform.position);

        if (dropObj != null && dropObj.TryGetComponent<DropItemController>(out var dropItem))
        {
            dropItem.OnGetFromPool();
        }
        else
        {
            Debug.LogWarning($"[OreController] 드롭 아이템 풀에서 꺼내기 실패 ID: {dropId}");
        }
    }

    public void Interact()
    {
    }

    public bool IsInteractable(Vector3 playerPos, float range, BoxCollider2D playerCollider)
    {
        if (_collider == null) return false;

        Vector2 playerPos2D = new Vector2(playerPos.x, playerPos.y);
        Vector2 closestPoint = _collider.ClosestPoint(playerPos2D);
        float centerToEdge = Vector2.Distance(playerPos2D, closestPoint);

        float playerRadius = playerCollider.size.magnitude * 0.5f * Mathf.Max(playerCollider.transform.lossyScale.x, playerCollider.transform.lossyScale.y);
        float edgeToEdgeDistance = Mathf.Max(0f, centerToEdge - playerRadius);

        return edgeToEdgeDistance <= 1.5f;
    }
}
