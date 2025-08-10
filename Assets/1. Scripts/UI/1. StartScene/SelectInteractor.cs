using UnityEngine;
using UnityEngine.EventSystems;

public class SelectInteractor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform _rect;
    
    private Vector2 _startPosition;
    private Vector2 _leftPosition;
    private Vector2 _rightPosition;

    private void Reset()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Calculate()
    {
        _startPosition = _rect.anchoredPosition + new Vector2(600f, 0f) + new Vector2(-136.005f, 260.025f);
        _leftPosition = _startPosition - new Vector2(_rect.rect.width * 0.5f, 0f);
        _rightPosition = _startPosition + new Vector2(_rect.rect.width * 0.5f, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Calculate();
        StartSceneManager.Instance.StartSceneUI.selectImage.ShowImage(_leftPosition, _rightPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartSceneManager.Instance.StartSceneUI.selectImage.HideImage();
    }
}
