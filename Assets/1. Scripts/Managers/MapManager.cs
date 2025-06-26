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
}
