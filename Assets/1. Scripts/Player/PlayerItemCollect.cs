using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollect : MonoBehaviour
{
    [SerializeField] private float pickupRadius = 3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float delayBeforeAttract = 0.5f;

    [Tooltip("감지할 드롭 아이템 레이어 이름들")]
    [SerializeField] private List<string> dropItemLayerNames;

    private LayerMask dropItemLayerMask;
    private float attractTimer = 0f;
    private bool canAttract = false;

    private void Awake()
    {
        dropItemLayerMask = 0;
        foreach (var layerName in dropItemLayerNames)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1)
            {
                Debug.LogWarning($"존재하지 않는 레이어 이름: {layerName}");
                continue;
            }
            dropItemLayerMask |= (1 << layer);
        }
    }

    private void Update()
    {
        if (!canAttract)
        {
            attractTimer += Time.deltaTime;
            if (attractTimer >= delayBeforeAttract)
            {
                canAttract = true;
            }
            else
            {
                return;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius, dropItemLayerMask);

        foreach (var hit in hits)
        {
            JewelController jewel = hit.GetComponent<JewelController>();
            if (jewel != null && jewel.IsMined())
            {
                MoveAndCollect(jewel.transform, jewel.GetId(), jewel.gameObject);
                continue;
            }

            DropItemController dropItem = hit.GetComponent<DropItemController>();
            if (dropItem != null)
            {
                MoveAndCollect(dropItem.transform, dropItem.GetId(), dropItem.gameObject);
            }
        }
    }

    private void MoveAndCollect(Transform itemTransform, int itemId, GameObject itemObject)
    {
        itemTransform.position = Vector3.MoveTowards(itemTransform.position, transform.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(itemTransform.position, transform.position) <= 0.1f)
        {
            if (itemObject.TryGetComponent(out JewelController jewel))
            {
                AudioManager.Instance.PlaySFX("JewelPickUp");
            }

            InventoryManager.Instance.Inventory.AddItem(itemId, 1);
            PoolManager.Instance.ReturnToPool(itemId, itemObject);
        }
    }
}