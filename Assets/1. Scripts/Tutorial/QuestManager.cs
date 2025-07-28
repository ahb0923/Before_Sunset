using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    [SerializeField] private List<Quest> quests;
    private int curIndex = 0;
    private bool isAllClear = false;

    private void Update()
    {
        // 퀘스트를 클리어하면 다음 퀘스트 진행
        if (quests[curIndex].isClear && !isAllClear)
        {
            NextQuest();
        }
    }

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
        }

        UIManager.Instance.QuestUI.UpdateQuestUI(curQuest);
    }

    /// <summary>
    /// 퀘스트 타입이 일치하면 퀘스트의 할당량을 올림
    /// </summary>
    public void AddQuestClearAmount(QUEST_TYPE type, int amount = 1)
    {
        if(type == quests[curIndex].Type)
        {
            quests[curIndex].AddAmount(amount);
        }
    }
}
