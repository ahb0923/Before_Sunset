using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildPreview : MonoBehaviour
{
    [SerializeField] public Tilemap previewTilemap;
    [SerializeField] public TileBase greenTile;
    [SerializeField] public TileBase redTile;
    [SerializeField] private LayerMask _layerMask;

    private void Awake()
    {
        //Instance = this;
        previewTilemap = GetComponent<Tilemap>();
    }

    /// <summary>
    /// 1) 마우스가 가리키는 위치, 설치하려는 건물의 사이즈, 설치하려는 tilemap 정보 받음<br/>
    /// 2) 해당 위치의 tilemap 에 tile 있는지, 해당 위치에 "Building" Layer를 가진 오브젝트가 없는지 체크<br/>
    /// 3) 조건이 통과된다면 초록색, 아니면 빨간색 tile 설치<br/>
    /// 4) 이중 반복문을 통해 x, y 좌표 전부다 계산 (설치 건물 크기가 크지 않아 0(n^2)라도 괜찮을 것 같음)
    /// </summary>
    /// <param name="worldPos">드래그 중 실시간으로 받아오는 마우스 포인터의 월드 위치</param>
    /// <param name="size">설치하려는 프리팹의 사이즈 (x, y)</param>
    /// <param name="groundTilemap">메인 맵의 tilemap 정보</param>
    public void ShowPreview(Vector3 worldPos, Vector2Int size)
    {
        previewTilemap.ClearAllTiles();

        int offsetX = size.x / 2;
        int offsetY = size.y / 2;

        Tilemap groundTilemap = DefenseManager.Instance.GroundTile;
        Vector3Int centerCell = groundTilemap.WorldToCell(worldPos);
        Vector3Int origin = centerCell - new Vector3Int(offsetX, offsetY, 0);

        if (!BuildManager.Instance.BuildableTiles.Contains(centerCell))
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3Int cell = origin + new Vector3Int(x, y, 0);

                    TileBase tileToShow = redTile;
                    previewTilemap.SetTile(cell, tileToShow);
                }
            }
        }
        else
        {
            // 현재 설치하려는 건물의 x와 y좌표
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    // 셀 중심좌표 구하기
                    Vector3Int cell = origin + new Vector3Int(x, y, 0);
                    Vector3 cellCenter = groundTilemap.CellToWorld(cell) + new Vector3(0.5f, 0.5f);

                    bool isValidTile = groundTilemap.HasTile(cell);
                    bool isBlocked = Physics2D.OverlapBox(cellCenter, Vector2.one * 0.9f, 0f, _layerMask);

                    TileBase tileToShow = (isValidTile && !isBlocked) ? greenTile : redTile;
                    previewTilemap.SetTile(cell, tileToShow);
                }
            }
        }
    }
    /// <summary>
    /// 설치된 타일 전부 제거
    /// </summary>
    public void Clear()
    {
        previewTilemap.ClearAllTiles();
    }
}