using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    [SerializeField] private List<Quest> quests;
    private int curIndex = 0;
    private bool isAllClear = false;

    [Header("Arrow 관련")]
    [SerializeField] private GuideArrow _arrow;
    public GuideArrow Arrow => _arrow;
    [SerializeField] private List<Transform> _targets;
    private int curTargetIndex = 0;

    private void Start()
    {
        if (_arrow == null) return;

        // 처음에 광산 입구 표시
        _arrow.SettingTarget(_targets[curTargetIndex]);
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

            // 타워 건설 전에는 필요 아이템 추가
            if(curQuest.InventoryItemId != -1)
            {
                InventoryManager.Instance.Inventory.AddItem(curQuest.InventoryItemId, curQuest.InventoryItemAmount);
            }

            // 코어 업그레이드 전에 정수 추가
            if(curQuest.Type == QUEST_TYPE.UpgradeCore)
            {
                UpgradeManager.Instance.AddEssencePiece(300);
            }

            // 광산 이동 퀘스트는 화살표 표시
            if(curQuest.Type == QUEST_TYPE.MoveToMine)
            {
                curTargetIndex++;
                if(curTargetIndex < _targets.Count)
                {
                    _arrow?.SettingTarget(_targets[curTargetIndex]);
                }
            }
        }

        UIManager.Instance.QuestUI.DisplayClear(curQuest);
    }

    /// <summary>
    /// 퀘스트 타입과 ID가 일치하면 퀘스트의 할당량을 올림
    /// </summary>
    public void AddQuestAmount(QUEST_TYPE type, int id = -1,int amount = 1)
    {
        if (!GameManager.Instance.IsTutorial) return;

        SetQuestAmount(type, id, quests[curIndex].CurAmount + amount);
    }

    /// <summary>
    /// 퀘스트 타입과 ID가 일치하면 퀘스트의 할당량을 설정
    /// </summary>
    public void SetQuestAmount(QUEST_TYPE type, int id = -1, int amount = 1)
    {
        if (!GameManager.Instance.IsTutorial) return;

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
