using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelController : MonoBehaviour, IPoolable
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

    public void OnMined()
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
    }
}
