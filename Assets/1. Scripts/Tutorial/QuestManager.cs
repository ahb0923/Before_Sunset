using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    [SerializeField] private List<Quest> quests;
    private int curIndex = 0;
    private bool isAllClear = false;

    /// <summary>
    /// 퀘스트 완료 시에 다음 퀘스트 진행
    /// </summary>
    [ContextMenu("다음 퀘스트")]
    private void NextQuest()
    {
        Quest curQuest;
        if(curIndex + 1 >= quests.Count)
        {
            isAllClear = true;
            curQuest = null;
        }
        else
        {
            curIndex++;
            curQuest = quests[curIndex];
            if(curQuest.InventoryItemId != -1)
            {
                InventoryManager.Instance.Inventory.AddItem(curQuest.InventoryItemId, curQuest.InventoryItemAmount);
            }
        }

        UIManager.Instance.QuestUI.DisplayClear(curQuest);
    }

    /// <summary>
    /// 퀘스트 타입과 ID가 일치하면 퀘스트의 할당량을 올림
    /// </summary>
    public void AddQuestAmount(QUEST_TYPE type, int id = -1,int amount = 1)
    {
        SetQuestAmount(type, id, quests[curIndex].CurAmount + amount);
    }

    /// <summary>
    /// 퀘스트 타입과 ID가 일치하면 퀘스트의 할당량을 설정
    /// </summary>
    public void SetQuestAmount(QUEST_TYPE type, int id = -1, int amount = 1)
    {
        if (type == quests[curIndex].Type && id == quests[curIndex].objectId)
        {
            quests[curIndex].SetAmount(amount);
            UIManager.Instance.QuestUI.UpdateQuestUI(quests[curIndex]);

            // 퀘스트를 클리어하면 다음 퀘스트 진행
            if (quests[curIndex].isClear && !isAllClear)
            {
                NextQuest();
            }
        }
    }
}
