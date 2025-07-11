[System.Serializable]
public class TimeSaveData
{
    public int stage;
    public int day;
    public float dailyTime;

    public TimeSaveData(int stage, int day, float dailyTime)
    {
        this.stage = stage;
        this.day = day;
        this.dailyTime = dailyTime;
    }
}
