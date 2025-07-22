using System.Collections.Generic;

[System.Serializable]
public class MineSaveData
{
    public int mapId;
    public List<ResourceSaveData> resources;

    public MineSaveData()
    {
        resources = new List<ResourceSaveData>();
    }
}

[System.Serializable]
public class ResourceSaveData
{
    public int resourceId;
    public SerializableVector3 position;
    public int curHp;

    public ResourceSaveData(int resourceId, SerializableVector3 position)
    {
        this.resourceId = resourceId;
        this.position = position;
    }
}
