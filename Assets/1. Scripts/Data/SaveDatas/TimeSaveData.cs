[System.Serializable]
public class TimeSaveData
{
    public int day;
    public float dailyTime;
    public bool isNight;

    public TimeSaveData(int day, float dailyTime, bool isNight)
    {
        this.day = day;
        this.dailyTime = dailyTime;
        this.isNight = isNight;
    }
}
