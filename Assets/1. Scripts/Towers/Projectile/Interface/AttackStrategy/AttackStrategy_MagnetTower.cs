using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStrategy_MagnetTower : IAttackStrategy
{
    public IEnumerator Attack(BaseTower tower)
    {
        float radius = tower.statHandler.AttackRange;
        float pullSpeed = tower.statHandler.AttackPower;
        Vector3 centerPos = tower.transform.position;


        tower.ui.animator.SetTrigger("IsAttack");
        AoeEffectManager.Instance.TriggerAOE(centerPos, new Color(255f / 255f, 0f / 255f, 255f / 255f, 50f / 255f), true);

        Collider2D[] hits = Physics2D.OverlapCircleAll(centerPos, radius, LayerMask.GetMask("Monster"));

        List<GameObject> candidates = new();
        foreach (var h in hits)
        {
            if (h == null || !h.gameObject.activeSelf) continue;
            candidates.Add(h.gameObject);
        }

        candidates.Sort((a, b) => Vector2.Distance(centerPos, b.transform.position).CompareTo(Vector2.Distance(centerPos, a.transform.position)));

        int pullCount = Mathf.Min(3, candidates.Count);
        for (int i = 0; i < pullCount; i++)
        {
            GameObject pullTarget = candidates[i];
            tower.StartCoroutine(PullTargetCoroutine(pullTarget, centerPos, pullSpeed));
        }

        yield return null;
    }

    private IEnumerator PullTargetCoroutine(GameObject target, Vector3 center, float speed)
    {
        while (target != null && target.activeSelf)
        {
            Vector3 dir = (center - target.transform.position).normalized;
            target.transform.position += dir * speed * Time.deltaTime;

            if (Vector3.Distance(center, target.transform.position) < 2f)
                break;

            yield return null;
        }
    }
}
