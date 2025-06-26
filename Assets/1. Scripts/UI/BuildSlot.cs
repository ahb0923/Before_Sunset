using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("[ 에디터 할당 ]")]
    // 실제 건물 데이터 스크립트가 attach 되어있는 prefab 정보
    [SerializeField] public BaseTower towerPrefab;
    [SerializeField] public Image slotIcon;

    [SerializeField]
    private bool _isPlacing;
    public bool isPlacing => _isPlacing;

    //[Header("[ 임시 테스트 용도 ]")]
    //[SerializeField] public Transform dragImage;
    //[SerializeField] public Tilemap tilemap;
    private void Awake()
    {
        _isPlacing = false;
    }

    private void Update()
    {
        // 아이템이 클릭된 상태인지 검사
        if (_isPlacing)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            MapManager.Instance.DragIcon.SetPosition(Input.mousePosition);
            MapManager.Instance.BuildPreview.ShowPreview(mouseWorld, towerPrefab.size);
        }
    }


    /// <summary>
    /// 1) <br/>
    /// 2) <br/>
    /// 3) 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    /// <summary>
    /// 1) <br/>
    /// 2) 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
    }

    /// <summary>
    /// 좌클릭 Drag 시작, 우클릭 취소
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 좌클릭시
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isPlacing)
            {
                Debug.Log("배치시작");
                // 배치 시작
                _isPlacing = true;
                MapManager.Instance.DragIcon.Show();
                MapManager.Instance.DragIcon.SetIcon(towerPrefab.icon.sprite);
            }
            else
            {
                Debug.Log("설치시작");
                // 배치 확정
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;

                BaseTower newTower = Instantiate(towerPrefab);
                if (!newTower.TryPlaceAt(worldPos))
                {
                    Destroy(newTower.gameObject);
                }

                CancelPlacement(); // 상태 초기화
            }
        }
        // 우클릭 시
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isPlacing)
            {
                CancelPlacement();
            }
           
        }
    }

    private void CancelPlacement()
    {
        _isPlacing = false;
        MapManager.Instance.DragIcon.Hide();
        MapManager.Instance.BuildPreview.Clear();
    }
    
}

