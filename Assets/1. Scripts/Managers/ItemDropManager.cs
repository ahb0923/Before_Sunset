using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDropManager : MonoSingleton<ItemDropManager>
{
    private float _elapsed = 0f;


    public void DropItem(int id, int amount, Transform dropTransform)
    {
        



        PoolManager.Instance.GetFromPool(id, dropTransform.position, transform);
    }

    public void Movement(GameObject item)
    {
        /*
        _elapsed += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(_elapsed / _duration);

        Vector3 projectilePosition = Vector3.Lerp(_start, _end, normalizedTime);
        projectilePosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * _maxHeight;
        _self.position = projectilePosition;

        Vector3 direction = _end - _start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _self.rotation = Quaternion.Euler(0, 0, angle - 90f);

        return normalizedTime >= 1f;*/
    }
}