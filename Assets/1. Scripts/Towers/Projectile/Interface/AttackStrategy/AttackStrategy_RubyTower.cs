using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStrategy_RubyTower : IAttackStrategy
{
    public IEnumerator Attack(BaseTower tower)
    {
        float radius = tower.statHandler.AttackRange;
        float pullSpeed = tower.statHandler.AttackPower;
        Vector3 centerPos = tower.transform.position;

        if (tower.ai.CurState == TOWER_STATE.Destroy)
            yield break;

        tower.ui.animator.SetTrigger("IsAttack");
        AoeEffectManager.Instance.TriggerAOE(centerPos, new Color(255f / 255f, 80f / 255f, 80f / 255f, 50f / 255f));

        Collider2D[] hits = Physics2D.OverlapCircleAll(centerPos, radius, LayerMask.GetMask("Monster"));

        List<GameObject> candidates = new();
        foreach (var h in hits)
        {
            if (h == null || !h.gameObject.activeSelf) continue;
            candidates.Add(h.gameObject);
        }

        foreach (var enemy in candidates)
        {
            if (!enemy.TryGetComponent(out BaseMonster monster)) continue;

            if (monster.HasDebuff(DEBUFF_TYPE.Burn)) continue;
            // if (monster.HasDebuff(DEBUFF_TYPE.Burn) && tower.statHandler.BuildType == TOWER_BUILD_TYPE.Base) continue;

            var debuffObject = PoolManager.Instance.GetFromPool((int)tower.statHandler.DebuffID, enemy.transform.position, enemy.transform);
            if (!debuffObject.TryGetComponent(out BaseDebuff debuff)) continue;

            debuff.Apply(monster);
        }

        yield return null;
    }
}


