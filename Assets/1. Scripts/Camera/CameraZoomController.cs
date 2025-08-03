using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;

    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float zoomSmoothSpeed = 0.2f;

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
}
