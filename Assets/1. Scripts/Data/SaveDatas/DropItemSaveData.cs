using UnityEngine;

[System.Serializable]
public class DropItemSaveData
{
    public int id;
    public SerializableVector3 position;

    public DropItemSaveData(int id, SerializableVector3 position)
    {
        this.id = id;
        this.position = position;
    }
}
