using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum BUILDING_TYPE
{
    Normal,
    Slow
}
public class BaseTower : MonoBehaviour
{
    [Header(" [에디터 할당] ")]
    [SerializeField] public SpriteRenderer icon;
    [SerializeField] public List<Sprite> constructionIcon;
    [SerializeField] public Vector2Int size = new Vector2Int(1, 1); // 1x1 또는 2x2 등

    [SerializeField] private Tilemap _groundTilemap;

    private Collider2D _myCollider;
    [SerializeField] private Collider2D _attackRangeCollider;
    [SerializeField] private LayerMask _layerMask;

    // state 는 ai로 접근
    public TowerAI ai;
    // stat Handler => 타워 스탯 관련 
    public TowerStatHandler statHandler;
    // 공격 센서
    public TowerAttackSensor attackSensor;
    // ui 관련
    //public TowerUI ui;
    // 발사체
    public GameObject projectile;

    private void Awake()
    {
        //if (ai == null)
        //    ai = GetComponent<BuildingAI>();

        if (statHandler == null)
            statHandler = new TowerStatHandler();
        statHandler.Init();

        if (attackSensor == null)
            attackSensor = GetComponentInChildren<TowerAttackSensor>();
        //attackSensor.Init();

        //if (ui == null)
        //     ui = GetComponentInChildren<BuildingUI>();
        //ui.Init();

        if (icon == null)
            icon = GetComponentInChildren<SpriteRenderer>();

        if (_myCollider == null)
            _myCollider = GetComponent<Collider2D>();

        _groundTilemap = MapManager.Instance.GroundTile;
    }
    public void SetTilemap(Tilemap tilemap)
    {
        this._groundTilemap = tilemap;
    }

    /// <summary>
    /// 1) tile의 유효성을 검사(tile이 존재하는지 & 설치가능한 지역인지)
    /// 2) Building이 이미 지어져 있는지 체크
    /// 3) 빈 공간 & 설치 가능 지역일 경우에만 건물 설치 허가
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns>설치 가능한지 판단</returns>
    public bool TryPlaceAt(Vector3 worldPos)
    {
        Vector3Int originCell = _groundTilemap.WorldToCell(worldPos);
        // 카메라가 현재 비추고 있는 시야영역
        BoundsInt visibleTiles = GetVisibleTileBounds(_groundTilemap);

        //Tile 유효성 검사
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int checkCell = originCell + new Vector3Int(x, y, 0);

                //화면 밖 설치 불가
                if (!visibleTiles.Contains(checkCell))
                {
                    Debug.Log($"설치 실패: 카메라 밖 타일 {checkCell}");
                    return false;
                }

                // 타일 맵 존재 유무 자체 + 그게 Ground 인지... 추후 Layer 별개 생성 및 이름 변경 필요
                if (!_groundTilemap.HasTile(checkCell) || _groundTilemap.gameObject.layer != LayerMask.NameToLayer("Ground"))
                {
                    Debug.Log($"[설치 실패] 타일 없음 => {checkCell}");
                    return false;
                }
            }

        //중심 위치 계산(맨 좌측하단의 픽셀의 월드 포인트로 부터 x의 반, y의 반 만큼 더함)
        Vector3 centerWorldPos = _groundTilemap.CellToWorld(originCell) + new Vector3(size.x / 2f, size.y / 2f, 0);

        // 자기 자신 컬라이더 끄기
        _myCollider.enabled = false;

        //충돌 검사 (검사하는 overlapBox 크기살짝 줄이기 => 붙여서 짓기 불가한 상황 해결)
        Collider2D hit = Physics2D.OverlapBox(centerWorldPos, size - new Vector2(0.1f, 0.1f), 0f, _layerMask);

        // 다시 on
        _myCollider.enabled = true;

        if (hit != null)
        {
            Debug.LogWarning($"[설치 실패] 이미 건물이 있음 => 『{hit.gameObject.name}』 에 충돌");
            return false;
        }

        if (!CheckResource())
        {
            return false;
        }


        //건물 배치
        transform.position = centerWorldPos;
        RenderUtil.SetSortingOrderByY(icon);

        return true;
    }
    /// <summary>
    /// 현재 viewport에 보이는(화면에 실제로 보이는) 위치의 worldPosition값 
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

    /// <summary>
    /// 추후 건물  설치시에 TryPlaceAt에서 혹은 드래그 시도시에 보유중인 자원량을 체크할 메서드
    /// </summary>
    /// <returns>설치에 필요한 자원이 충분한지 판단</returns>
    public bool CheckResource()
    {
        /*
        if (!GameManager.Instance.Storage.HasEnoughResource(RESOURCE_TYPE.Gold, statHandler.RequireGold))
        {
            Debug.LogWarning("[설치 실패] : 『Gold』가 부족합니다");
            return false;
        }
        if (!GameManager.Instance.Storage.HasEnoughResource(RESOURCE_TYPE.Wood, statHandler.RequireWood))
        {
            Debug.LogWarning("[설치 실패] : 『Wood』가 부족합니다");
            return false;
        }

        GameManager.Instance.Storage.UseResource(RESOURCE_TYPE.Gold, statHandler.RequireGold);
        GameManager.Instance.Storage.UseResource(RESOURCE_TYPE.Wood, statHandler.RequireWood);
        */
        return true;
    }

    //전처리기 Gizmo
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_groundTilemap == null) return;

        Vector3Int originCell = _groundTilemap.WorldToCell(transform.position - new Vector3(size.x / 2f, size.y / 2f));
        Vector3 center = _groundTilemap.CellToWorld(originCell) + new Vector3(size.x / 2f, size.y / 2f, 0);
        Vector2 areaSize = size;

        bool isPlaceable = !Physics2D.OverlapBox(center, areaSize, 0f, LayerMask.GetMask("Building"));

        Gizmos.color = isPlaceable ? Color.green : Color.red;
        Gizmos.DrawWireCube(center, areaSize);
    }
#endif
}
