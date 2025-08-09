using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragIcon : MonoBehaviour
{
    public SpriteRenderer iconImage;
    public SpriteRenderer attackAreaImage;

    private void Reset()
    {
        iconImage = Helper_Component.FindChildComponent<SpriteRenderer>(transform, "PreviewImage");
        attackAreaImage = Helper_Component.FindChildComponent<SpriteRenderer>(transform, "AreaImage");
    }

    private void Awake()
    {
        Hide();
    }

    public void SetIcon(Sprite sprite, float areaRadius, Color areaColor)
    {
        attackAreaImage.gameObject.SetActive(true);
        areaRadius += 0.5f;
        iconImage.sprite = sprite;
        attackAreaImage.transform.localScale = new Vector3(areaRadius, areaRadius, 1);
        attackAreaImage.color = areaColor;
    }
    public void SetIcon(Sprite sprite)
    {
        iconImage.sprite = sprite;
        attackAreaImage.gameObject.SetActive(false);
    }

    public void SetPosition(Vector2 screenPos)
    {
        // 이미지 크기 체크 필요
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);// + new Vector3(0.5f, 1.5f); ;
        worldPos.z = 0;
        transform.position = worldPos;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
