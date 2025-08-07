[System.Serializable]
public class SmelterSaveData
{
    public int smelterId;
    public SerializableVector3 position;
    public ItemSaveData inputItem;
    public float smeltedTime;
    public ItemSaveData outputItem;

    public SmelterSaveData(int smelterId, SerializableVector3 position, ItemSaveData inputItem, float smeltedTime, ItemSaveData outputItem)
    {
        this.smelterId = smelterId;
        this.position = position;
        this.inputItem = inputItem;
        this.smeltedTime = smeltedTime;
        this.outputItem = outputItem;
    }
}
