using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnectingManager : MonoSingleton<LineConnectingManager>
{
    [SerializeField] private GameObject electricLinePrefab;
    [SerializeField] private Transform lineParent;

    public GameObject CreateLineBetween(ElectriclineTower a, ElectriclineTower b)
    {
        var from = a.transform.position;
        var to = b.transform.position;
        var prefabId = Helper_Component.GetComponent<Electricline>(electricLinePrefab).Id;
        GameObject lineObj = PoolManager.Instance.GetFromPool(prefabId, (from + to) / 2f, lineParent);

        SetupLineTransform(lineObj.transform, from, to);

        // 공격력 계산 및 전달
        if (lineObj.TryGetComponent(out Electricline line))
        {
            float atkA = a.GetBaseTower().statHandler.AttackPower;
            float atkB = b.GetBaseTower().statHandler.AttackPower;
            line.SetAttackPower((atkA + atkB) / 2f);

            line.SetOwners(a, b);
        }

        return lineObj;
    }

    public void ReturnLineToPool(GameObject lineObj)
    {
        PoolManager.Instance.ReturnToPool(Helper_Component.GetComponent<Electricline>(electricLinePrefab).Id, lineObj);
    }

    private void SetupLineTransform(Transform lineTransform, Vector3 from, Vector3 to)
    {
        Vector3 mid = (from + to) / 2f;
        Vector3 dir = (to - from).normalized;
        float length = Vector3.Distance(from, to);

        lineTransform.position = mid;
        lineTransform.right = dir;
        lineTransform.localScale = Vector3.one;

        if (lineTransform.TryGetComponent(out Electricline line))
        {
            line.SetLength(length);
        }
    }
}
