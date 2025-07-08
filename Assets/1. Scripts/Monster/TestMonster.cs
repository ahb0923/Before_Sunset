using UnityEngine;

public class TestMonster : MonoBehaviour, IDamageable
{
    public int maxHp = 100;
    public int curHp;

    private void Awake()
    {
        curHp = maxHp;
    }
    
    public void OnDamaged(Damaged damaged)
    {
        if (damaged.Attacker == null)
        {
            Debug.LogWarning("타격 대상 못찾음!");
            return;
        }

        int realDamage = DamageCalculator.CalcDamage(damaged.Value, 0f, damaged.IgnoreDefense);
        curHp -= realDamage;
        curHp = Mathf.Max(curHp, 0);

        Debug.Log($"테스트 몬스터 : {curHp + realDamage} -> {curHp}");

        if (curHp == 0)
        {
            Debug.Log("테스트 몬스터 사망!");
        }
    }
}
