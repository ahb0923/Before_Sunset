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

    /// <summary>
    /// 1) 하이라이트 생각한다면 이곳에서<br/>
    /// 2) 추후 사용할 일 없으면 삭제
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    /// <summary>
    /// 1) 하이라이트 적용시켰다면 나갈경우 이곳에서 해제<br/>
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {

    }

    /// <summary>
    /// 좌클릭, 우클릭으로 설치할 건물 마우스 포인트에 탈부착
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 좌클릭시
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!BuildManager.Instance.IsPlacing)
            {
                BuildManager.Instance.StartPlacing(towerPrefab);
            }
        }
        // 우클릭시
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (BuildManager.Instance.IsPlacing)
            {
                BuildManager.Instance.CancelPlacing();
            }
        }
    }
    
}

