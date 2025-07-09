using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RecallUIHandler : MonoBehaviour
{
    public static RecallUIHandler Instance;
    public event Action OnCountdownFinished;

    [Header("UI References")]
    [SerializeField] private GameObject recallIconGroup;
    [SerializeField] private Image holdCircleImage;
    [SerializeField] private Slider countdownSlider;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 sliderOffset = new Vector3(0, -1f, 0);

    private Canvas canvas;

    private void Awake()
    {
        Instance = this;

        if (recallIconGroup != null)
            recallIconGroup.SetActive(false);

        if (countdownSlider != null)
            countdownSlider.gameObject.SetActive(false);

        canvas = GetComponentInParent<Canvas>();

        if (playerTransform == null)
            Debug.LogWarning("RecallUIHandler: PlayerTransform이 할당되지 않았습니다!");
    }

    private void Update()
    {
        if (playerTransform != null && countdownSlider != null && countdownSlider.gameObject.activeSelf)
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
        if (recallIconGroup != null)
        {
            recallIconGroup.SetActive(true);
            if (holdCircleImage != null)
                holdCircleImage.fillAmount = 0f;
        }
    }

    public void HideRecallIcon()
    {
        if (recallIconGroup != null)
            recallIconGroup.SetActive(false);
    }

    public void UpdateHoldProgress(float progress)
    {
        if (holdCircleImage != null)
            holdCircleImage.fillAmount = Mathf.Clamp01(progress);
    }

    public void StartRecallCountdown()
    {
        if (countdownSlider != null)
        {
            countdownSlider.gameObject.SetActive(true);
            StartCoroutine(UpdateCountdown());
        }
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
        if (recallIconGroup != null)
            recallIconGroup.SetActive(false);

        if (countdownSlider != null)
            countdownSlider.gameObject.SetActive(false);

        if (holdCircleImage != null)
            holdCircleImage.fillAmount = 0f;
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
}
