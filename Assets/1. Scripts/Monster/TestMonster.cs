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
            Debug.LogWarning("Ÿ�� ��� ��ã��!");
            return;
        }

        int realDamage = DamageCalculator.CalcDamage(damaged.Value, 0f, damaged.IgnoreDefense);
        curHp -= realDamage;
        curHp = Mathf.Max(curHp, 0);

        Debug.Log($"�׽�Ʈ ���� : {curHp + realDamage} -> {curHp}");

        if (curHp == 0)
        {
            Debug.Log("�׽�Ʈ ���� ���!");
        }
    }
}
