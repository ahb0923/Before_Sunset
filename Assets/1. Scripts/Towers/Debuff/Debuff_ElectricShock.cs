using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff_ElectricShock : BaseDebuff
{
    private float multiplier;

    protected override IEnumerator C_Effect()
    {
        var data = DataManager.Instance.DebuffData.GetById(DebuffId);
        multiplier = 1f + (data.effectValue / 100f);

        target.OnBeforeDamaged += AmplifyDamage;

        yield return new WaitForSeconds(duration);

        target.OnBeforeDamaged -= AmplifyDamage;

        Remove();
        PoolManager.Instance.ReturnToPool(DebuffId, gameObject);
    }

    private void AmplifyDamage(Damaged dmg)
    {
        dmg.Multiplier *= multiplier;
    }
}
