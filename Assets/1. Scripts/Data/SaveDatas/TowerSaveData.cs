using UnityEngine;

[System.Serializable]
public class TowerSaveData
{
    public int towerId;
    public SerializableVector3 position;
    public BUILDING_TYPE upgrade;
    public int curHp;

    public TowerSaveData(int towerId, Vector3 position, BUILDING_TYPE upgrade, int curHp)
    {
        this.towerId = towerId;
        this.position = position;
        this.upgrade = upgrade;
        this.curHp = curHp;
    }
}
