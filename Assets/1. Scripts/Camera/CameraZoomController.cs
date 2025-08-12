using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float zoomSmoothSpeed = 0.2f;

    [Header("Focus Settings")]
    [SerializeField] private float focusZoom = 5f;
    [SerializeField] private float focusDuration = 5f;

    private float _targetZoom;
    private float _currentZoom;

    private void Start()
    {
        if (virtualCam == null)
        {
            Debug.LogWarning("Virtual Camera 연결되지않음");
            return;
        }

        _currentZoom = virtualCam.m_Lens.OrthographicSize;
        _targetZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
        virtualCam.m_Lens.OrthographicSize = _targetZoom;
    }

    private void Update()
    {
        if (virtualCam == null) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            _targetZoom -= scroll * zoomSpeed * 0.1f;
            _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
        }

        _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, zoomSmoothSpeed);

        var lens = virtualCam.m_Lens;
        lens.OrthographicSize = _currentZoom;
        virtualCam.m_Lens = lens;
    }

    public void FocusGameOver(Transform target)
    {
        if (virtualCam == null || target == null) return;
        StopAllCoroutines();
        StartCoroutine(GameOverRoutine(target));
    }

    private IEnumerator GameOverRoutine(Transform target)
    {
        virtualCam.Follow = target;

        float startZoom = virtualCam.m_Lens.OrthographicSize;
        float endZoom = focusZoom;
        float zoomTime = focusDuration;
        float timer = 0f;

        // BGM 서서히 꺼지는건데 게임오버 느낌 납니다.
        AudioManager.Instance.FadeOutBGM(focusDuration);

        while (timer < zoomTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / zoomTime);

            float smoothT = t * t * (3f - 2f * t);

            var lens = virtualCam.m_Lens;
            lens.OrthographicSize = Mathf.Lerp(startZoom, endZoom, smoothT);
            virtualCam.m_Lens = lens;

            yield return null;
        }
    }
}
