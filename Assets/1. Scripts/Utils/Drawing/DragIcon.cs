using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragIcon : MonoBehaviour
{
    public SpriteRenderer iconImage;

    private void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponent<SpriteRenderer>();
        Hide();
    }

    public void SetIcon(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }

    public void SetPosition(Vector2 screenPos)
    {
        // 이미지 크기 체크 필요
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos) + new Vector3(0.5f, 1.5f); ;
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
