using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class BuildManager : MonoSingleton<BuildManager>
{
    [Header("설치")]
    [SerializeField] private LayerMask _buildingLayer;
    [SerializeField] private Transform _buildablePool;
    public Transform BuildablePool => _buildablePool;

    [Header("하이라이트")]
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;

    private Tilemap _groundTilemap;
    private GameObject _buildPrefab;
    private BuildInfo _buildInfo;
    private bool _isPlacing;
    public bool IsPlacing => _isPlacing;

    // 파괴 선택(추후 프로퍼티로 변경)
    public bool IsOnDestroy = false;

    private readonly HashSet<Vector3Int> _buildableTiles = new();
    private readonly List<Vector3Int> _highlightedTiles = new();

    private float _lastRadius = -1f;

    private void Start()
    {
        if (_groundTilemap == null)
            _groundTilemap = DefenseManager.Instance.GroundTile;
    }

    /// <summary>
    /// 버튼 클릭했을때 아이콘이 마우스 포인터따라오게 해당되는 prefab.Image
    /// </summary>
    /// <param name="prefab"></param>
    public void StartPlacing(int ID)
    {
        var towerPrefab = DataManager.Instance.TowerData.GetPrefabById(ID);
        var smelterPrefab = DataManager.Instance.SmelterData.GetPrefabById(ID);
        _buildPrefab = towerPrefab ?? smelterPrefab;
        if (_buildPrefab == null)
        {
            Debug.LogError($"[BuildManager] ID {ID}에 해당하는 프리팹 없음");
            return;
        }

        _buildInfo = Helper_Component.GetComponentInChildren<BuildInfo>(_buildPrefab);
        _isPlacing = true;

        DefenseManager.Instance.DragIcon.Show();
        DefenseManager.Instance.DragIcon.SetIcon(_buildInfo.spriteRenderer.sprite);

        CalculateBuildableTiles(_groundTilemap, DefenseManager.Instance.Core.transform, DefenseManager.Instance.Core.GetLightAreaRadius());
        ShowHighlightTiles();
    }
    
    public void CancelPlacing()
    {
        _isPlacing = false;
        _buildInfo = null;
        _buildPrefab = null;

        DefenseManager.Instance.DragIcon.Hide();
        DefenseManager.Instance.BuildPreview.Clear();

        ClearHighlights();
    }
    
    private void Update()
    {
        // 이미지를 드래그중인 상황이 아니라면 update 실행x => 성능 개선용
        if (!_isPlacing || _buildInfo == null)
            return;

        // 마우스 위치
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector3Int cell = _groundTilemap.WorldToCell(mouseWorld);
        Vector3 cellCenter = _groundTilemap.GetCellCenterWorld(cell);

        // 프리뷰 표시 (셀 중심으로)
        DefenseManager.Instance.DragIcon.SetPosition(Camera.main.WorldToScreenPoint(cellCenter));
        DefenseManager.Instance.BuildPreview.ShowPreview(cellCenter, _buildInfo.buildSize);
        // 프리뷰 표시 (마우스 포인터)
        //DefenseManager.Instance.DragIcon.SetPosition(Input.mousePosition);
        //DefenseManager.Instance.BuildPreview.ShowPreview(mouseWorld, _buildInfo.buildSize);

        // 좌클릭 확정(UI 위에 클릭시 설치 안되도록 조건 추가)
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
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
        if (TryBuildTower(_buildInfo, worldPos))
        {
            // 추후 개선 요구 이름 포함으로 찾는거 위험함
            if (_buildPrefab.name.Contains("Tower"))
                AudioManager.Instance.PlaySFX("CreateTower");
            else if (_buildPrefab.name.Contains("Smelter"))
                AudioManager.Instance.PlaySFX("CreateSmelter");
        }
        else { ToastManager.Instance.ShowToast("해당 구역에는 설치할 수 없습니다."); }

        CancelPlacing();
        UIManager.Instance.CraftArea.Open();
    }

    /// <summary>
    /// 1) tile의 유효성을 검사(tile이 존재하는지 & 설치가능한 지역인지)
    /// 2) Building이 이미 지어져 있는지 체크
    /// 3) 빈 공간 & 설치 가능 지역일 경우에만 건물 설치 허가
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public bool TryBuildTower(BuildInfo prefab, Vector3 worldPos)
    {
        Vector3Int originCell = _groundTilemap.WorldToCell(worldPos);

        // 셀의 중심이 타일 안에 있는지 확인
        Vector3Int centerCell = _groundTilemap.WorldToCell(worldPos);
        if (!_buildableTiles.Contains(centerCell))
            return false;

        // 중앙 위치
        Vector3 cellCenter = _groundTilemap.GetCellCenterWorld(originCell);

        // 충돌 검사
        Collider2D hit = Physics2D.OverlapBox(cellCenter, prefab.buildSize - new Vector2(0.1f, 0.1f), 0f, _buildingLayer);
        if (hit != null)
        {
            Debug.Log($"설치 실패: {hit.name}과 충돌");
            return false;
        }

        if (!GameManager.Instance.GOD_MODE)
        {
            // 인벤토리 재화 사용
            var towerDb = DataManager.Instance.TowerData.GetById(_buildInfo.id);
            if (towerDb != null)
            {
                foreach (var item in towerDb.buildRequirements)
                    InventoryManager.Instance.Inventory.UseItem(item.Key, item.Value);
            }
            else
            {
                var smelterDb = DataManager.Instance.SmelterData.GetById(_buildInfo.id);
                foreach (var item in smelterDb.buildRequirements)
                    InventoryManager.Instance.Inventory.UseItem(item.Key, item.Value);
            }
        }


        // 배치 성공
        var obj = PoolManager.Instance.GetFromPool(prefab.id, cellCenter, _buildablePool);
        DefenseManager.Instance.AddObstacle(obj.transform, 1); // 일단 1x1이니까 1로 두었음
        return true;
    }

    /// <summary>
    /// 중심과 반경으로 설치 가능 타일 계산
    /// </summary>
    private void CalculateBuildableTiles(Tilemap tilemap, Transform target, float radius)
    {
        _buildableTiles.Clear();
        _highlightedTiles.Clear();

        Debug.Log(target.position);

        Vector3 centerWorld = target.position;
        float sqrRadius = radius * radius;
        Vector3Int centerCell = tilemap.WorldToCell(centerWorld);
        int cellRadius = Mathf.CeilToInt(radius);

        for (int x = -cellRadius; x <= cellRadius; x++)
        {
            for (int y = -cellRadius; y <= cellRadius; y++)
            {
                Vector3Int cell = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);

                Vector3 cellCenter = tilemap.GetCellCenterWorld(cell);
                float distSqr = (cellCenter - centerWorld).sqrMagnitude;

                if (distSqr <= sqrRadius && tilemap.HasTile(cell))
                {
                    _buildableTiles.Add(cell);
                    _highlightedTiles.Add(cell);
                }
            }
        }
    }

    /// <summary>
    /// 설치 가능 타일 하이라이트 표시
    /// </summary>
    private void ShowHighlightTiles()
    {
        if (highlightTilemap == null || highlightTile == null) return;

        foreach (var cell in _highlightedTiles)
        {
            highlightTilemap.SetTile(cell, highlightTile);
        }
    }

    /// <summary>
    /// 기존 하이라이트 제거
    /// </summary>
    private void ClearHighlights()
    {
        if (highlightTilemap == null) return;

        foreach (var cell in _highlightedTiles)
        {
            highlightTilemap.SetTile(cell, null);
        }
        _highlightedTiles.Clear();
    }
}
