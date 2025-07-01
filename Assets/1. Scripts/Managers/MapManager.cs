using UnityEngine.Tilemaps;

public class MapManager : MonoSingleton<MapManager>
{
    // 맵 매니저 하위 오브젝트

    public Tilemap GroundTile { get; private set; }
    public BuildPreview BuildPreview { get; private set; }
    public DragIcon DragIcon { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        GroundTile = GetComponentInChildren<Tilemap>();
        BuildPreview = GetComponentInChildren<BuildPreview>();
        DragIcon = GetComponentInChildren<DragIcon>(true);
    }

}
