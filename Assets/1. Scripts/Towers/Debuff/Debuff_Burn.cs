using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff_Burn : BaseDebuff
{
    protected override IEnumerator C_Effect()
    {
        var data = DataManager.Instance.DebuffData.GetById(DebuffId);
        float damage = data.effectValue;
        float duration = data.duration;
        //float duration = 5;

        int tickCount = (int)duration;
        //float interval = duration / tickCount;

        for (int i = 0; i < tickCount; i++)
        {
            if (target == null || !target.gameObject.activeInHierarchy) break;

            DamagedSystem.Instance.Send(new Damaged
            {
                Attacker = gameObject,
                Victim = target.gameObject,
                Value = damage,
                IgnoreDefense = false,
                Multiplier = 1.0f
            });
            yield return Helper_Coroutine.WaitSeconds(1.0F);
        }

        Remove(); 
        PoolManager.Instance.ReturnToPool(DebuffId, gameObject); 
    }
}
