using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraZoomController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 3f;
    public float maxZoom = 10f;

    [Header("Smooth Zoom")]
    public float zoomSmoothSpeed = 0.2f;

    private float targetZoom;
    private float currentZoom;

    private void Start()
    {
        if (virtualCam == null)
        {
            Debug.LogWarning("Virtual Camera 연결되지않음.");
            return;
        }

        currentZoom = virtualCam.m_Lens.OrthographicSize;
        targetZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        virtualCam.m_Lens.OrthographicSize = targetZoom;
    }

    private void Update()
    {
        if (virtualCam == null) return;

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetZoom -= scroll * zoomSpeed * 0.1f;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSmoothSpeed);
        var lens = virtualCam.m_Lens;
        lens.OrthographicSize = currentZoom;
        virtualCam.m_Lens = lens;
    }
}
