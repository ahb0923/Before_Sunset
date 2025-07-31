using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDragConnector : MonoSingleton<LineDragConnector>
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private float maxDistance = 8.5f;

    private ElectriclineTower startTower;
    private bool isDragging = false;

    public bool IsDragging => isDragging;

    private void Update()
    {
        if (!isDragging) return;

        Vector3 startPos = startTower.transform.position;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, mouseWorld);

        if (Input.GetMouseButtonDown(0))  // 마우스 다시 눌렀을 때 연결 시도
        {
            TryConnectToTarget(mouseWorld);
            CancelDrag();
        }
        else if (Input.GetMouseButtonDown(1))  // 우클릭 취소
        {
            ToastManager.Instance.ShowToast("연결 취소됨");
            CancelDrag();
        }
    }

    public void BeginDrag(ElectriclineTower tower)
    {
        if (isDragging || tower.IsConnected) return;

        startTower = tower;
        isDragging = true;

        lineRenderer.positionCount = 2;
        lineRenderer.gameObject.SetActive(true);
    }

    private void TryConnectToTarget(Vector3 mousePos)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePos, towerLayer);

        if (hit && hit.TryGetComponent(out ElectriclineTower targetTower))
        {
            if (targetTower == startTower)
            {
                ToastManager.Instance.ShowToast("자기 자신과 연결할 수 없습니다.");
                return;
            }

            if (startTower.IsConnected || targetTower.IsConnected)
            {
                ToastManager.Instance.ShowToast("이미 연결된 타워가 있습니다.");
                return;
            }

            float dist = Vector3.Distance(startTower.transform.position, targetTower.transform.position);
            if (dist > maxDistance)
            {
                ToastManager.Instance.ShowToast($"연결 거리가 너무 멉니다: {dist:F2} > {maxDistance}");
                return;
            }

            bool success = startTower.TryConnectTo(targetTower);
            ToastManager.Instance.ShowToast(success ? "전깃줄 연결 성공!" : "연결 실패");
        }
        else
        {
            ToastManager.Instance.ShowToast("연결 가능한 타워가 아닙니다.");
        }
    }

    private void CancelDrag()
    {
        isDragging = false;
        startTower = null;
        lineRenderer.gameObject.SetActive(false);
    }
}