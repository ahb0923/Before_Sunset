using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelController : MonoBehaviour
{
    public PickUpItem pickUpItem;
    public Collider2D _collider;

    private bool mined = false;

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
