using System.Collections.Generic;
using UnityEngine;

public class ItemToastUI : MonoBehaviour
{
    [SerializeField] private GameObject _toastPrefab;
    [SerializeField] private int _initialCount = 10;

    private Queue<GameObject> _toastPool = new Queue<GameObject>();
    private Queue<GameObject> _activeToasts = new Queue<GameObject>();

    private const string TOAST_PREFAB = "UI/ItemToast";
    
    private void Reset()
    {
        _toastPrefab = Resources.Load<GameObject>(TOAST_PREFAB);
    }

    private void Awake()
    {
        InitToastPool();
    }

    private void InitToastPool()
    {
        for (int i = 0; i < _initialCount; i++)
        {
            var toast = Instantiate(_toastPrefab, this.transform);
            _toastPool.Enqueue(toast);
            toast.SetActive(false);
        }
    }
    
    public void ShowToast(string itemName)
    {
        var toast = GetToast();
        var toastSlot = toast.GetComponent<ItemToast>();
        toastSlot.ShowToast(itemName);
    }

    private GameObject GetToast()
    {
        if (_toastPool.Count > 0)
        {
            var t = _toastPool.Dequeue();
            _activeToasts.Enqueue(t);
            t.gameObject.SetActive(true);
            return t;
        }
        else
        {
            var t = _activeToasts.Dequeue();
            _activeToasts.Enqueue(t);
            return t;
        }
    }

    public void ReturnToast(GameObject toast)
    {
        toast.gameObject.SetActive(false);
        _toastPool.Enqueue(toast);
    }
}
