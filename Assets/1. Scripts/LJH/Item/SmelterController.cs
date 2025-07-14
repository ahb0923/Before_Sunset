using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmelterController : MonoBehaviour
{
    public SmelterDatabase smelterData;
    public SmelterUI smelterUI;

    private Item currentInputItem;
    private bool isSmelting = false;
    private float smeltingTime = 5f;  // 제련 소요 시간 (초)
    private int remainingToSmelt = 0;
    private float timer = 0f;

    private Inventory inventory;

    private void Awake()
    {
        inventory = InventoryManager.Instance.Inventory;
    }

    private void Update()
    {
        if (isSmelting)
        {
            // 입력 아이템이 없거나 개수가 0이면 제련 중단
            if (currentInputItem == null || currentInputItem.stack <= 0)
            {
                Debug.LogWarning("제련 도중 입력 아이템이 없어서 중단합니다.");
                StopSmelting();
                return;
            }

            timer += Time.deltaTime;
            if (timer >= smeltingTime)
            {
                FinishSmelting();
            }
        }
    }

    /// <summary>
    /// 제련 시작: 인벤토리에서 아이템을 받아 처리
    /// </summary>
    /// <param name="inputItem">제련할 아이템 인스턴스</param>
    /// <returns>성공 여부</returns>
    public bool StartSmelting(Item inputItem)
    {
        Debug.Log($"StartSmelting called with id:{inputItem.Data.id}, qty:{inputItem.stack}");

        if (isSmelting)
        {
            Debug.LogWarning("이미 제련 중입니다.");
            return false;
        }

        if (!smelterData.smeltingIdList.Contains(inputItem.Data.id))
        {
            Debug.LogWarning("이 제련소에서 제련할 수 없는 아이템입니다.");
            return false;
        }

        // 입력 아이템 복제 (원본과 참조 분리)
        currentInputItem = new Item(inputItem.Data)
        {
            stack = inputItem.stack
        };

        remainingToSmelt = currentInputItem.stack;

        smelterUI.smelterInputSlot.SetItem(currentInputItem);
        smelterUI.smelterOutputSlot.ClearSlot();

        timer = 0f;
        isSmelting = true;

        return true;
    }

    /// <summary>
    /// 제련 1회 완료 처리
    /// </summary>
    private void FinishSmelting()
    {
        timer = 0f;

        if (currentInputItem == null || currentInputItem.stack <= 0)
        {
            StopSmelting();
            return;
        }

        var mineralData = currentInputItem.Data as MineralDatabase;
        if (mineralData == null)
        {
            Debug.LogError("입력 아이템이 제련 가능한 광석 데이터가 아닙니다.");
            StopSmelting();
            return;
        }

        int ingotId = mineralData.ingotId;
        var outputData = DataManager.Instance.ItemData.GetId(ingotId);

        // 출력 슬롯에 아이템 추가: 1개씩 누적
        Item outputItem = smelterUI.smelterOutputSlot.CurrentItem;

        if (outputItem == null)
        {
            outputItem = new Item(outputData)
            {
                stack = 1
            };
            smelterUI.smelterOutputSlot.SetItem(outputItem);
        }
        else if (outputItem.Data.id == ingotId)
        {
            outputItem.stack += 1;
            smelterUI.smelterOutputSlot.RefreshUI();
        }
        else
        {
            Debug.LogWarning("출력 슬롯에 다른 아이템이 있어서 제련을 멈춥니다.");
            StopSmelting();
            return;
        }

        remainingToSmelt--;
        currentInputItem.stack--;
        smelterUI.smelterInputSlot.RefreshUI();

        if (remainingToSmelt <= 0)
        {
            StopSmelting();
        }
        else
        {
            // 계속 제련 진행
            isSmelting = true;
        }
    }

    /// <summary>
    /// 출력 슬롯 아이템을 인벤토리에 받음
    /// </summary>
    public void ReceiveOutput()
    {
        if (smelterUI.smelterOutputSlot.IsOutputEmpty())
        {
            Debug.LogWarning("받을 아이템이 없습니다.");
            return;
        }

        Item outputItem = smelterUI.smelterOutputSlot.CurrentItem;
        inventory.AddItem(outputItem.Data.id, outputItem.stack);

        smelterUI.smelterOutputSlot.ClearSlot();
    }


    /// <summary>
    /// 제련 중지, 상태 초기화
    /// </summary>
    private void StopSmelting()
    {
        isSmelting = false;
        timer = 0f;
        remainingToSmelt = 0;
        currentInputItem = null;
        smelterUI.smelterInputSlot.ClearSlot();
    }
}