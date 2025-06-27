public enum MONSTER_TYPE
{
    Melee,
    Ranged,
    Tank,
}

[System.Serializable]
public class MonsterData
{
    public int id;
    public string monsterName;
    public MONSTER_TYPE type;

    public int hp;
    public int damage;
    public float aps;
    public float speed;
    public float range;
}
