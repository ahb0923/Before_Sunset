using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraZoomController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;

    public float zoomSpeed = 10f;
    public float minZoom = 3f;
    public float maxZoom = 10f;

    private void Start()
    {
        var lens = virtualCam.m_Lens;
        lens.OrthographicSize = Mathf.Clamp(lens.OrthographicSize, minZoom, maxZoom);
        virtualCam.m_Lens = lens;
    }

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (virtualCam == null || scroll == 0f) return;

        var lens = virtualCam.m_Lens;

        float targetZoom = lens.OrthographicSize - scroll * zoomSpeed * 0.1f; // 부드럽게 조정
        float newZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        lens.OrthographicSize = newZoom;
        virtualCam.m_Lens = lens;
    }
}
