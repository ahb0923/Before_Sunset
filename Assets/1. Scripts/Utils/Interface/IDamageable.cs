
using UnityEngine;

public interface IDamageable
{
    void OnDamaged(Damaged damaged);
}

public class Damaged
{
    public float Value;
    public GameObject Attacker;
    public GameObject Victim;
    public bool IgnoreDefense;

    public float Multiplier;
    public float FinalDamage => Value * Multiplier;

    public override string ToString()
    {
        return $"Damage: {Value}, Mult: {Multiplier}, Attacker: {Attacker}, Victim: {Victim}, IgnoreDefense: {IgnoreDefense}";
    }
}