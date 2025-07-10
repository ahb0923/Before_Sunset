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
    [SerializeField] private Slider countdownSlider;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 sliderOffset = new Vector3(0, -1f, 0);

    private void Reset()
    {
        // _recallIconProgressBar = Helper_Component.FindChildComponent<Image>(this.transform.parent)
    }

    private void Awake()
    { 
        countdownSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (countdownSlider.gameObject.activeSelf)
        {
            UpdateSliderPosition();
        }
    }

    private void UpdateSliderPosition()
    {
        Vector3 worldPos = playerTransform.position + sliderOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        countdownSlider.transform.position = screenPos;
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
        countdownSlider.gameObject.SetActive(true);
        StartCoroutine(UpdateCountdown());
    }

    private IEnumerator UpdateCountdown()
    {
        float duration = 3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (countdownSlider != null)
                countdownSlider.value = 0f + (elapsed / duration);
            yield return null;
        }
        if (countdownSlider != null)
            countdownSlider.gameObject.SetActive(false);

        OnCountdownFinished?.Invoke();
    }

    public void ResetUI()
    {
        countdownSlider.gameObject.SetActive(false);
        _recallIconProgressBar.fillAmount = 0f;
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
}
