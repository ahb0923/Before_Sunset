using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BuildManager : MonoSingleton<BuildManager>
{
    private Tilemap _groundTilemap;
    private BaseTower _towerPrefab;

    private Collider2D _towerMainCollider;

    [SerializeField] private LayerMask _buildingLayer;
    [SerializeField] private bool _isPlacing;
    public bool IsPlacing => _isPlacing;


    private void Start()
    {
        if (_groundTilemap == null)
            _groundTilemap = MapManager.Instance.GroundTile;
    }

    public void StartPlacing(BaseTower prefab)
    {
        _towerPrefab = prefab;
        _towerMainCollider = prefab.GetComponent<Collider2D>();
        Debug.Log(_towerMainCollider);
        _isPlacing = true;

        MapManager.Instance.DragIcon.Show();
        MapManager.Instance.DragIcon.SetIcon(prefab.icon.sprite);
    }

    public void CancelPlacing()
    {
        _isPlacing = false;
        _towerPrefab = null;

        MapManager.Instance.DragIcon.Hide();
        MapManager.Instance.BuildPreview.Clear();
    }
    private void Update()
    {
        // 이미지를 드래그중인 상황이 아니라면 update 실행x => 성능 개선용
        if (!_isPlacing || _towerPrefab == null)
            return;

        // 마우스 위치
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        // 프리뷰 표시
        MapManager.Instance.DragIcon.SetPosition(Input.mousePosition);
        MapManager.Instance.BuildPreview.ShowPreview(mouseWorld, _towerPrefab.size);

        // 좌클릭 확정
        if (Input.GetMouseButtonDown(0)) //  && !EventSystem.current.IsPointerOverGameObject()
        {
            TryPlace(mouseWorld);
        }
        // 우클릭 취소
        else if (Input.GetMouseButtonDown(1))
        {
            CancelPlacing();
        }
    }
    private void TryPlace(Vector3 worldPos)
    {
        if (TryBuildTower(_towerPrefab, worldPos))
        {
            Debug.Log("설치 성공");
        }
        else { Debug.Log("설치 실패"); }

        CancelPlacing();
    }
    /// <summary>
    /// 1) tile의 유효성을 검사(tile이 존재하는지 & 설치가능한 지역인지)
    /// 2) Building이 이미 지어져 있는지 체크
    /// 3) 빈 공간 & 설치 가능 지역일 경우에만 건물 설치 허가
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public bool TryBuildTower(BaseTower prefab, Vector3 worldPos)
    {
        Vector3Int originCell = _groundTilemap.WorldToCell(worldPos);
        BoundsInt visibleTiles = GetVisibleTileBounds(_groundTilemap);

        // 모든 타일이 화면 안에 있고, 존재하는지 확인
        for (int x = 0; x < prefab.size.x; x++)
            for (int y = 0; y < prefab.size.y; y++)
            {
                Vector3Int checkCell = originCell + new Vector3Int(x, y, 0);
                if (!visibleTiles.Contains(checkCell) || !_groundTilemap.HasTile(checkCell))
                    return false;
            }

        // 중앙 위치
        Vector3 cellCenter = _groundTilemap.GetCellCenterWorld(originCell);

        // 충돌 검사
        Collider2D hit = Physics2D.OverlapBox(cellCenter, prefab.size - new Vector2(0.1f, 0.1f), 0f, _buildingLayer);
        if (hit != null)
        {
            Debug.Log($"설치 실패: {hit.name}과 충돌");
            return false;
        }

        // 배치 성공
        BaseTower tower = Instantiate(prefab, cellCenter, Quaternion.identity);
        MapManager.Instance.AddObstacle(tower.transform, 1); // 일단 1x1이니까 1로 두었음
        RenderUtil.SetSortingOrderByY(tower.icon);

        return true;
    }
    /// <summary>
    /// 현재 viewport에 보이는(화면에 실제로 보이는) 위치의 worldPosition값<br/>
    /// 추후에 『카메라의 비추는 전체 범위 => 플레이어의 시야 범위』로 변경할 경우 코드 수정
    /// </summary>
    /// <returns></returns>
    private BoundsInt GetVisibleTileBounds(Tilemap tilemap)
    {
        Camera cam = Camera.main;
        float distance = -cam.transform.position.z;

        Vector3 worldMin = cam.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 worldMax = cam.ViewportToWorldPoint(new Vector3(1, 1, distance));

        Vector3Int minCell = tilemap.WorldToCell(worldMin);
        Vector3Int maxCell = tilemap.WorldToCell(worldMax);

        Vector3Int size = maxCell - minCell + Vector3Int.one;

        return new BoundsInt(minCell, size);
    }
}
