using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStrategy_HealTower : IAttackStrategy
{
    public IEnumerator Attack(BaseTower tower)
    {
        float radius = tower.statHandler.AttackRange;
        int healAmount = Mathf.RoundToInt(tower.statHandler.AttackPower);
        Vector3 centerPos = tower.transform.position;

        if (tower.ai.CurState == TOWER_STATE.Destroy)
            yield break;

        tower.ui.animator.SetTrigger("IsAttack");
        EffectManager.Instance.TriggerAOE(centerPos, new Color(50f / 200f, 80f / 255f, 50f / 255f, 50f / 255f), tower.statHandler.AttackRange + 0.5f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(tower.transform.position, radius, LayerMask.GetMask("Tower"));

        List<GameObject> healables = new();
        foreach (var hit in hits)
        {
            if (hit.gameObject == tower.gameObject) continue;

            var towerAI = hit.GetComponent<TowerAI>();
            if (towerAI != null && towerAI.CurState == TOWER_STATE.Destroy) continue;

            var hitStat = Helper_Component.GetComponent<TowerStatHandler>(hit);
            if (hitStat != null && hitStat.CurrHp < hitStat.MaxHp)
            {
                healables.Add(hit.gameObject);
            }
        }

        healables.Sort((a, b) =>
        {
            var sa = a.GetComponent<TowerStatHandler>();
            var sb = b.GetComponent<TowerStatHandler>();
            float ra = sa == null ? float.MaxValue : sa.CurrHp;
            float rb = sb == null ? float.MaxValue : sb.CurrHp;
            return ra.CompareTo(rb);
        });

        int healCount = Mathf.Min(3, healables.Count);
        for (int i = 0; i < healCount; i++)
        {
            var healStat = healables[i].GetComponent<TowerStatHandler>();
            healStat.CurrHp += healAmount;

            //Debug.Log($"힐타워: {healables[i].name} 체력 {healAmount} 회복");
        }

        yield return null;
    }
}

