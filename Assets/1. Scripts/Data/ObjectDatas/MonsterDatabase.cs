public enum ATTACK_TYPE
{
    Melee,
    Ranged,
}

public enum MOVE_TYPE
{
    Ground,
    Air,
}

[System.Serializable]
public class MonsterDatabase
{
    public int id;
    public string monsterName;
    public ATTACK_TYPE attackType;
    public MOVE_TYPE moveType;

    public int hp;
    public int damage;
    public float aps;
    public float speed;
    public float range;
    public int size;

    public string context;
}
