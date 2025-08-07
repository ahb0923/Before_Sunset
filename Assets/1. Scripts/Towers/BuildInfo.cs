using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildInfo : MonoBehaviour, IBuildable
{
    public int id;
    public Vector2Int buildSize = new Vector2Int(1, 1); // 1x1 또는 2x2 등
    public SpriteRenderer spriteRenderer;
    private void Awake()
    {

    }
    private void Start()
    {
        if(spriteRenderer == null)
            spriteRenderer = Helper_Component.GetComponentInChildren<SpriteRenderer>(gameObject);
    }

    public void Init(int id, Vector2Int buildSize)
    {
        this.id = id;
        //this.buildSize = buildSize;
    }

    public void SettingSpriteOrder()
    {
        RenderUtil.SetSortingOrderByY(spriteRenderer);
    }

    public Vector2 GetSize()
    {
        throw new System.NotImplementedException();
    }
}
