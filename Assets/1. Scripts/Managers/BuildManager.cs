using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BuildManager : MonoSingleton<BuildManager>
{
    [SerializeField] private LayerMask _buildingLayer;
    [SerializeField] private Transform _buildablePool;

    private Tilemap _groundTilemap;
    private GameObject _buildPrefab;
    private BuildInfo _buildInfo;
    private bool _isPlacing;
    public bool IsPlacing => _isPlacing;


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
        _buildPrefab = DataManager.Instance.TowerData.GetPrefabById(ID) ?? _buildPrefab;
        _buildPrefab = DataManager.Instance.SmelterData.GetPrefabById(ID) ?? _buildPrefab;


        _buildInfo = Helper_Component.GetComponentInChildren<BuildInfo>(_buildPrefab);
        _isPlacing = true;

        DefenseManager.Instance.DragIcon.Show();
        DefenseManager.Instance.DragIcon.SetIcon(_buildInfo.spriteRenderer.sprite);
    }

    public void CancelPlacing()
    {
        _isPlacing = false;
        _buildInfo = null;

        DefenseManager.Instance.DragIcon.Hide();
        DefenseManager.Instance.BuildPreview.Clear();
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
        // 프리뷰 표시
        //DefenseManager.Instance.DragIcon.SetPosition(Input.mousePosition);
        //DefenseManager.Instance.BuildPreview.ShowPreview(mouseWorld, _buildInfo.buildSize);

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
        if (TryBuildTower(_buildInfo, worldPos))
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
    public bool TryBuildTower(BuildInfo prefab, Vector3 worldPos)
    {
        Vector3Int originCell = _groundTilemap.WorldToCell(worldPos);
        BoundsInt visibleTiles = GetVisibleTileBounds(_groundTilemap);

        // 모든 타일이 화면 안에 있고, 존재하는지 확인
        for (int x = 0; x < prefab.buildSize.x; x++)
            for (int y = 0; y < prefab.buildSize.y; y++)
            {
                Vector3Int checkCell = originCell + new Vector3Int(x, y, 0);
                if (!visibleTiles.Contains(checkCell) || !_groundTilemap.HasTile(checkCell))
                    return false;
            }

        // 중앙 위치
        Vector3 cellCenter = _groundTilemap.GetCellCenterWorld(originCell);

        // 충돌 검사
        Collider2D hit = Physics2D.OverlapBox(cellCenter, prefab.buildSize - new Vector2(0.1f, 0.1f), 0f, _buildingLayer);
        if (hit != null)
        {
            Debug.Log($"설치 실패: {hit.name}과 충돌");
            return false;
        }

        // 인벤토리 재화 체크
        // => 이곳에 코드 구현

        // 배치 성공
        var obj = PoolManager.Instance.GetFromPool(prefab.id, cellCenter, _buildablePool);
        DefenseManager.Instance.AddObstacle(obj.transform, 1); // 일단 1x1이니까 1로 두었음
        QuestManager.Instance?.AddQuestClearAmount(QUEST_TYPE.PlaceBuilding);
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
