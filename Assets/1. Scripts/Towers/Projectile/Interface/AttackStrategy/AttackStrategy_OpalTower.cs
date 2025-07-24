using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStrategy_OpalTower : IAttackStrategy
{
    private List<int> debuffIdList = new();

    public IEnumerator Attack(BaseTower tower)
    {
        float radius = tower.statHandler.AttackRange;
        float pullSpeed = tower.statHandler.AttackPower;
        Vector3 centerPos = tower.transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(centerPos, radius, LayerMask.GetMask("Monster"));

        List<GameObject> candidates = new();
        foreach (var h in hits)
        {
            if (h == null || !h.gameObject.activeSelf) continue;
            candidates.Add(h.gameObject);
        }

        // 디버프 리스트, 현재 데이터 없어서 하드코딩
        if (debuffIdList.Count == 0)
        {
            if (tower.statHandler.ID == 420)
            {
                debuffIdList.Add(1400);
                debuffIdList.Add(1402);
                debuffIdList.Add(1404);
            }
            else if (tower.statHandler.ID == 421)
            {
                debuffIdList.Add(1401);
                debuffIdList.Add(1403);
                debuffIdList.Add(1405);
            }
            else Debug.Log("ID가 맞지 않음");
        }
        
        if (debuffIdList.Count > 0)
        {
            int randomIndex = Random.Range(0, debuffIdList.Count); 
            int selectedDebuffId = debuffIdList[randomIndex];


            // 테스트용 이펙트
            Color originalColor = tower.ui.effectArea.color;
            if (tower.ui.effectArea != null)
            {
                switch (selectedDebuffId)
                {
                    case 1400:
                    case 1401:
                        tower.ui.effectArea.color = ColorExtensions.WithAlpha(Color.red, 100f / 255f);
                        break;
                    case 1402:
                    case 1403:
                        tower.ui.effectArea.color = ColorExtensions.WithAlpha(Color.blue, 100f / 255f);
                        break ;
                    case 1404:
                    case 1405:
                        tower.ui.effectArea.color = ColorExtensions.WithAlpha(Color.yellow, 100f / 255f);
                        break;
                }
            }

            foreach (var enemy in candidates)
            {
                if (enemy.TryGetComponent(out BaseMonster monster))
                {
                    if(monster.isDebuffed == true)
                    {
                        continue;
                    }
                }

                var debuffObject = PoolManager.Instance.GetFromPool(selectedDebuffId, enemy.transform.position, enemy.transform);

                if (debuffObject.TryGetComponent(out BaseDebuff debuff) && monster != null)
                {
                    debuff.Apply(monster);
                }
            }
        }
        
        yield return new WaitForSeconds(tower.statHandler.AttackSpeed);
    }
}


