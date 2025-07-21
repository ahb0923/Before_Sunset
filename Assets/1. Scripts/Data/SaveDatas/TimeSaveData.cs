[System.Serializable]
public class TimeSaveData
{
    public int stage;
    public int day;
    public float dailyTime;
    public bool isNight;

    public TimeSaveData(int stage, int day, float dailyTime, bool isNight)
    {
        this.stage = stage;
        this.day = day;
        this.dailyTime = dailyTime;
        this.isNight = isNight;
    }
}
