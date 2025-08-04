using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelController : MonoBehaviour, IPoolable, IInteractable, IResourceStateSavable
{
    public Collider2D _collider;

    private bool mined = false;

    [SerializeField] private int _id;
    public int GetId() => _id;

    public bool IsMined() => mined;

    public void OnInstantiate()
    {
        //
    }

    public void OnGetFromPool()
    {
        mined = false;
        _collider.isTrigger = false;
    }

    public void OnReturnToPool()
    {
        mined = true;
        _collider.isTrigger = true;
    }

    public ResourceState SaveState()
    {
        return new ResourceState
        {
            Id = _id,
            Position = transform.position,
            IsMined = mined
        };
    }

    public void LoadState(ResourceState state)
    {
        transform.position = state.Position;
        mined = state.IsMined;

        if (_collider != null)
            _collider.isTrigger = mined;
    }

    public void Interact()
    {
        if (mined) return;
        mined = true;
        if (_collider != null)
        {
            _collider.isTrigger = true;
        }

        transform.SetParent(MapManager.Instance.ItemParent);
    }

    public bool IsInteractable(Vector3 playerPos, float range, BoxCollider2D playerCollider)
    {
        Vector2 playerPos2D = new Vector2(playerPos.x, playerPos.y);
        Vector2 closestPoint = _collider.ClosestPoint(playerPos2D);
        float centerToEdge = Vector2.Distance(playerPos2D, closestPoint);

        float playerRadius = playerCollider.size.magnitude * 0.5f * Mathf.Max(playerCollider.transform.lossyScale.x, playerCollider.transform.lossyScale.y);
        float edgeToEdgeDistance = Mathf.Max(0f, centerToEdge - playerRadius);

        return edgeToEdgeDistance <= 1.5f;
    }

    public int GetObejctSize()
    {
        return 1;
    }
}
