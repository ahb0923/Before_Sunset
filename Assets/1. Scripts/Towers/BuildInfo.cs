using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildInfo : MonoBehaviour
{
    public int id;
    public Vector2Int size = new Vector2Int(1, 1); // 1x1 또는 2x2 등
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        if(spriteRenderer == null)
            spriteRenderer = Helper_Component.GetComponentInChildren<SpriteRenderer>(gameObject);
    }

    public void Init(int id, Vector2Int size)
    {
        this.id = id;
        this.size = size;
    }

    public void SettingSpriteOrder()
    {
        RenderUtil.SetSortingOrderByY(spriteRenderer);
    }

}
