using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("스포너 컴포넌트")]
    [SerializeField] private OreSpawner oreSpawner;
    [SerializeField] private JewelSpawner jewelSpawner;

    [Header("스폰 데이터 리스트")]
    [SerializeField] private List<OreData> oreDataList;
    [SerializeField] private List<JewelData> jewelDataList;

    [Header("현재 스테이지")]
    [SerializeField] private int currentStage = 1;

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        if (oreSpawner != null)
            oreSpawner.SpawnResources(oreDataList, currentStage);

        if (jewelSpawner != null)
            jewelSpawner.SpawnResources(jewelDataList, currentStage);
    }

    // 스테이지 변경 시 스폰 다시할 때 호출
    public void ChangeStage(int newStage)
    {
        currentStage = newStage;
        ClearAll();
        SpawnAll();
    }

    // 기존에 생성된 오브젝트 정리
    private void ClearAll()
    {
        ClearChildren(oreSpawner?.transform);
        ClearChildren(jewelSpawner?.transform);
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
}
