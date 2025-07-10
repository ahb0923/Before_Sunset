using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RecallUI : MonoBehaviour
{
    public event Action OnCountdownFinished;

    [Header("UI References")]
    [SerializeField] private Image _recallIconProgressBar;
    [SerializeField] private Image _recallProgressBar;
    [SerializeField] private GameObject _recallBar;

    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _recallOffset = new Vector3(0, -1f, 0);

    private RectTransform _rect;
    
    private const string RECALL_ICON_PROGRESS_BAR = "RecallIconProgressBar";
    private const string RECALL_PROGRESS_BAR = "RecallProgressBar";
    private const string RECALL_BAR = "RecallBar";
    
    private void Reset()
    {
        _recallIconProgressBar = Helper_Component.FindChildComponent<Image>(this.transform.parent, RECALL_ICON_PROGRESS_BAR);
        _recallProgressBar = Helper_Component.FindChildComponent<Image>(this.transform, RECALL_PROGRESS_BAR);
        _recallBar = Helper_Component.FindChildGameObjectByName(this.gameObject, RECALL_BAR);
        _playerTransform = GameObject.Find("Player").transform;
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        CloseRecall();
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            UpdateSliderPosition();
        }
    }

    public void OpenRecall()
    {
        _rect.OpenAtCenter();
    }

    public void CloseRecall()
    {
        _recallIconProgressBar.fillAmount = 0f;
        _rect.CloseAndRestore();
    }

    private void UpdateSliderPosition()
    {
        Vector3 worldPos = _playerTransform.position + _recallOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        _recallBar.transform.position = screenPos;
    }

    public void ShowRecallIcon()
    {
        _recallIconProgressBar.fillAmount = 0f;
    }

    public void UpdateHoldProgress(float progress)
    {
        _recallIconProgressBar.fillAmount = Mathf.Clamp01(progress);
    }

    public void StartRecallCountdown()
    {
        OpenRecall();
        StartCoroutine(C_UpdateCountdown());
    }

    private IEnumerator C_UpdateCountdown()
    {
        float duration = 3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _recallProgressBar.fillAmount = 0f + (elapsed / duration);
            yield return null;
        }
        OnCountdownFinished?.Invoke();
    }
}
