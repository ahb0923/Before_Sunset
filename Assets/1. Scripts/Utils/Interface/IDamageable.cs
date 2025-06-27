
using UnityEngine;

public interface IDamageable
{
    void OnDamaged(Damaged damaged);
}

public struct Damaged
{
    public float Value;
    public GameObject Attacker;
    public GameObject Victim;
    public bool IgnoreDefense;

    public override string ToString()
    {
        return $"Damage: {Value}, Attacker: {Attacker}, Victim: {Victim}, IgnoreDefense: {IgnoreDefense}";
    }
}