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

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0f)
        {
            Debug.Log("스크롤: " + scroll);
        }

        if (virtualCam != null)
        {
            float size = virtualCam.m_Lens.OrthographicSize;
            size -= scroll * zoomSpeed * Time.deltaTime;
            size = Mathf.Clamp(size, minZoom, maxZoom);
            virtualCam.m_Lens.OrthographicSize = size;

            Debug.Log("줌 사이즈: " + size);
        }
    }
}
