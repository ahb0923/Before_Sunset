using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Electricline : MonoBehaviour, IPoolable
{
    private int id = 10000;
    public int Id => id;

    private float _attackPower;
    private ElectriclineTower _ownerA;
    private ElectriclineTower _ownerB;

    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private SpriteRenderer _sprite;

    private Dictionary<GameObject, float> _lastTickTimes = new();

    public void SetAttackPower(float power) => _attackPower = power;

    public void SetOwners(ElectriclineTower a, ElectriclineTower b)
    {
        _ownerA = a;
        _ownerB = b;
    }
    public void SetLength(float length)
    {
        if (_sprite != null)
            _sprite.size = new Vector2(length, _sprite.size.y);

        if (TryGetComponent(out BoxCollider2D col))
            col.size = new Vector2(length, col.size.y);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!IsInMonsterLayer(other.gameObject)) return;

        float lastTick = _lastTickTimes.TryGetValue(other.gameObject, out var t) ? t : 0f;
        if (Time.time - lastTick < tickInterval) return;

        _lastTickTimes[other.gameObject] = Time.time;

        // 데미지
        DamagedSystem.Instance.Send(new Damaged
        {
            Attacker = gameObject,
            Victim = other.gameObject,
            Value = _attackPower,
            IgnoreDefense = false,
            Multiplier = 1.0f
        });

        // 넉백
        if (other.TryGetComponent(out Rigidbody2D rb))
        {
            Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
            float knockbackDistance = 0.5f;

            rb.DOMove(rb.position + knockbackDir * knockbackDistance, 0.2f)
                .SetEase(Ease.OutExpo);
        }
    }

    private bool IsInMonsterLayer(GameObject obj)
    {
        return ((1 << obj.layer) & monsterLayer) != 0;
    }

    public void NotifyOwnerDestroyed(ElectriclineTower destroyedOwner)
    {
        if (destroyedOwner == _ownerA || destroyedOwner == _ownerB)
        {
            LineConnectingManager.Instance.ReturnLineToPool(gameObject);
        }
    }

    public int GetId() => id;

    public void OnInstantiate() { }

    public void OnGetFromPool()
    {
        _lastTickTimes.Clear();
    }

    public void OnReturnToPool()
    {
        _attackPower = 0f;
        _ownerA = null;
        _ownerB = null;
        _lastTickTimes.Clear();
    }
}
