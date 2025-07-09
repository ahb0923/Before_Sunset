using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public int itemId;
    public float pickupRadius = 3f;
    public float moveSpeed = 5f;
    public float delayBeforeAttract = 1f;

    private Transform playerTransform;
    private float timer = 0f;
    private bool canAttract = false;

    private void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delayBeforeAttract)
        {
            canAttract = true;
        }

        if (!canAttract) return;
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= pickupRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            if (distance <= 0.1f)
            {
                //InventoryManager.Instance.Inventory.AddItem(itemId, 1);
                Destroy(gameObject);
            }
        }
    }
}