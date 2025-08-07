using System.Collections;
using UnityEngine;

public class ItemDropManager : MonoSingleton<ItemDropManager>, ISaveable
{
    private float _duration = 1.0f;
    [SerializeField] public float offsetRange = 0.05f;
    [SerializeField] public float maxHeight = 1.0f;

    /// <summary>
    /// 아이템 드롭 로직, 현재는 무조건 곡사 드롭으로 밖에 불가능
    /// 이동 방법 다르게 바꾸고 싶으면, 열거형 제작후 해당 열거형에 맞는 이동방식으로 액션 호출 혹은 델리게이트 사용
    /// </summary>
    /// <param name="id">떨굴 아이템의 id</param>
    /// <param name="amount">떨굴 아이템의 총량</param>
    /// <param name="dropTransform">떨어지기 시작하는 위치</param>
    public void DropItem(int id, int amount, Transform dropTransform, bool isMove = true)
    {
        for(int i=0; i<amount; i++)
        {
            GameObject dropObject = PoolManager.Instance.GetFromPool(id, dropTransform.position, transform);

            if (isMove)
            {
                Vector3 startPosition = dropTransform.position;
                Vector3 endPosition = startPosition + new Vector3(Random.Range(-offsetRange, offsetRange), Random.Range(-offsetRange, offsetRange), 0f);


                StartCoroutine(C_Movement(dropObject.transform, startPosition, endPosition));
            }
        }
    }


    private IEnumerator C_Movement(Transform obj, Vector3 startPos, Vector3 endPos)
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

    /// <summary>
    /// 드랍 아이템 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        foreach (var obj in transform.GetComponentsInChildren<Transform>())
        {
            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                DropItemSaveData item = new DropItemSaveData(poolable.GetId(), obj.position);
                data.dropItems.Add(item);
            }
        }
    }

    /// <summary>
    /// 드랍 아이템 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        // 떨어져 있는 드랍 아이템들 비활성화
        foreach (var obj in transform.GetComponentsInChildren<Transform>())
        {
            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                PoolManager.Instance.ReturnToPool(poolable.GetId(), obj.gameObject);
            }
        }

        // 로드한 드랍 아이템 활성화
        foreach (var item in data.dropItems)
        {
            GameObject obj = PoolManager.Instance.GetFromPool(item.id, item.position, transform);
        }
    }
}