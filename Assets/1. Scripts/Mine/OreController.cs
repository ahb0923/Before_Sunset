using System.Collections.Generic;
using UnityEngine;

public class OreController : MonoBehaviour, IPoolable, IInteractable
{
    public OreDatabase _data { get; private set; }

    public BasePlayer player;

    private int _currentHP;

    [SerializeField] private int _id;
    public int GetId() => _id;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void Init(BasePlayer basePlayer)
    {
        player = basePlayer;
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
        //throw new System.NotImplementedException();
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
        int pickaxePower = player.Stat.Pickaxe.crushingForce;
        int damage = player.Stat.Pickaxe.damage;

        if (!CanBeMined(pickaxePower))
        {
            Debug.Log("곡괭이 힘이 부족합니다.");
            return;
        }

        bool destroyed = Mine(damage);
    }

    public bool IsInteractable(Vector3 playerPos, float range, CircleCollider2D playerCollider)
    {
        if (_collider == null || playerCollider == null)
            return false;

        Vector2 playerPos2D = new Vector2(playerPos.x, playerPos.y);
        Vector2 closestPointToPlayer = _collider.ClosestPoint(playerPos2D);
        float centerToEdge = Vector2.Distance(playerPos2D, closestPointToPlayer);

        float playerRadius = playerCollider.radius * Mathf.Max(playerCollider.transform.lossyScale.x, playerCollider.transform.lossyScale.y);
        float edgeToEdgeDistance = Mathf.Max(0f, centerToEdge - playerRadius);

        return edgeToEdgeDistance <= range;
    }
}
