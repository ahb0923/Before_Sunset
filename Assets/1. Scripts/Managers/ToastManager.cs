using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ToastManager : MonoSingleton<ToastManager>
{
    [SerializeField] private GameObject _toastPrefab;
    [SerializeField] private int _initialCount = 10;

    private Queue<GameObject> toastPool = new Queue<GameObject>();

    private const string TOAST_PREFAB = "UI/ToastContainer";
    
    private void Reset()
    {
        _toastPrefab = Resources.Load<GameObject>(TOAST_PREFAB);
    }

    protected override void Awake()
    {
        base.Awake();
        InitToastPool();
    }

    private void InitToastPool()
    {
        for (int i = 0; i < _initialCount; i++)
        {
            var toast = Instantiate(_toastPrefab, this.transform);
            toastPool.Enqueue(toast);
        }
    }
    
    public void ShowToast(string message)
    {
        var toast = GetToast();
        var toastUI = toast.GetComponent<ToastUI>();
        toastUI.ShowToast(message);
    }

    private GameObject GetToast()
    {
        if (toastPool.Count > 0)
        {
            var t = toastPool.Dequeue();
            t.gameObject.SetActive(true);
            return t;
        }

        return Instantiate(_toastPrefab, this.transform);
    }

    public void ReturnToast(GameObject toast)
    {
        toast.gameObject.SetActive(false);
        toastPool.Enqueue(toast);
    }
}
