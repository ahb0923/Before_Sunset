using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelController : MonoBehaviour, IPoolable, IInteractable
{
    public PickUpItem pickUpItem;
    public Collider2D _collider;

    private bool mined = false;

    [SerializeField] private int _id;
    public int GetId() => _id;

    public void OnInstantiate()
    {
        //
    }

    public void OnGetFromPool()
    {
        mined = false;
        if (pickUpItem != null)
            pickUpItem.enabled = false;
        if (_collider != null)
            _collider.enabled = true;
    }

    public void OnReturnToPool()
    {
        mined = false;
        if (pickUpItem != null)
            pickUpItem.enabled = false;
        if (_collider != null)
            _collider.enabled = false;
    }

    public void Interact()
    {
        if (mined) return;

        mined = true;

        if (pickUpItem != null)
        {
            pickUpItem.enabled = true;
        }
        if (_collider != null)
        {
            _collider.enabled = false;
        }

        Debug.Log("쥬얼 파괴됨!");
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
