using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoSingleton<MapManager>
{
    // 맵 매니저 하위 오브젝트

    public Tilemap GroundTile { get; private set; }

    public BuildPreview BuildPreview { get; private set; }
     
    public DragIcon DragIcon { get; private set; }

    private void Start()
    {
        GroundTile = GetComponentInChildren<Tilemap>();
        Debug.Log(GroundTile);
        BuildPreview = GetComponentInChildren<BuildPreview>();
        Debug.Log(BuildPreview);
        DragIcon = GetComponentInChildren<DragIcon>(true);
        Debug.Log(DragIcon);
    }

}
