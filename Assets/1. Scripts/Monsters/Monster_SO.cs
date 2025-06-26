using UnityEngine;

public enum MONSTER_TYPE
{
    Melee,
    Ranged,
    Tank,
    // 필요 시 몬스터 타입 추가
}

[CreateAssetMenu(fileName = "Monster_", menuName = "Scriptable Objects/Monster")]
public class Monster_SO : ScriptableObject
{
    [field: SerializeField] public string MonsterName { get; private set; }
    [field: SerializeField] public MONSTER_TYPE Type { get; private set; }
    [field: SerializeField] public int Hp { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public float Aps { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public int Range { get; private set; }
    [field: SerializeField] public int Size { get; private set; }
}
