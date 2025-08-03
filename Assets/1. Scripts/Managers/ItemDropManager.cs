using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDropManager : MonoSingleton<ItemDropManager>
{
    private float _duration = 1.0f;
    [SerializeField] public float offsetRange = 0.05f;
    [SerializeField] public float maxHeight = 1.0f;


    public void DropItem(int id, int amount, Transform dropTransform)
    {
        for(int i=0; i<amount; i++)
        {
            GameObject dropObject = PoolManager.Instance.GetFromPool(id, dropTransform.position, transform);

            Vector3 startPosition = dropTransform.position;
            Vector3 endPosition = startPosition + new Vector3(Random.Range(-offsetRange, offsetRange), Random.Range(-offsetRange, offsetRange), 0f);




            StartCoroutine(C_Movement(dropObject.transform, startPosition, endPosition));
        }
    }

    public IEnumerator C_Movement(Transform obj, Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0f;

        while(elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _duration);

            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            pos.y += Mathf.Sin(t * Mathf.PI) * maxHeight;

            obj.position = pos;

            yield return null;
        }
        obj.position = endPos;
    }
}