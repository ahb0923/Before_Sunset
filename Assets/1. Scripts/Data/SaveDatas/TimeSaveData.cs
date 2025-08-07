[System.Serializable]
public class TimeSaveData
{
    public int day;
    public float dailyTime;

    public TimeSaveData(int day, float dailyTime)
    {
        this.day = day;
        this.dailyTime = dailyTime;
    }
}
