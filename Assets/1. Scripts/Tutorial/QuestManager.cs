using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    [SerializeField] private List<Quest> quests;
    private int curIndex = 0;
    private bool isAllClear = false;
    private Quest _curQuest => curIndex == -1 ? null : quests[curIndex];
    private Dictionary<QUEST_TYPE, bool> _clearQuestDict;

    [SerializeField] private List<Transform> _portals;
    private GuideArrow _guideArrow;

    protected override void Awake()
    {
        base.Awake();

        if(quests != null && quests.Count > 0)
        {
            _clearQuestDict = new Dictionary<QUEST_TYPE, bool>();
            foreach(Quest quest in quests)
            {
                _clearQuestDict[quest.Type] = false;
            }
        }
    }

    private void Start()
    {
        // 첫 퀘스트이므로 true
        if (_clearQuestDict != null)
            _clearQuestDict[_curQuest.Type] = true;

        _guideArrow = MapManager.Instance.Player.GetComponentInChildren<GuideArrow>();
        SetArrowTargetIndex(0);
    }

    /// <summary>
    /// 퀘스트 완료 시에 다음 퀘스트 진행
    /// </summary>
    [ContextMenu("다음 퀘스트")]
    private void NextQuest()
    {
        if (curIndex + 1 >= quests.Count)
        {
            curIndex = -1;
            isAllClear = true;
        }
        else
        {
            curIndex++;
            _clearQuestDict[_curQuest.Type] = true;

            // 퀘스트 수행 전에는 필요 아이템 추가
            foreach(ProvidedItem need in _curQuest.ProvidedItems)
            {
                InventoryManager.Instance.Inventory.AddItem(need.itemId, need.itemAmount);
            }

            // 코어 업그레이드 전에 정수 추가
            if(_curQuest.Type == QUEST_TYPE.UpgradeCore)
            {
                UpgradeManager.Instance.AddEssencePiece(300);
            }

            // 두번째 광산 입장 퀘스트는 2번 포탈로 설정 으로 설정
            if (_curQuest.Type == QUEST_TYPE.MoveToMine)
            {
                SetArrowTargetIndex(1);
            }
            else
            {
                _guideArrow.SettingTarget(null);
            }
        }

        UIManager.Instance.QuestUI.DisplayClear(_curQuest);
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

    /// <summary>
    /// 이전에 퀘스트를 클리어했거나, 현재 퀘스트일 경우에만 액션이 가능하도록 판단<br/>
    /// ※ 튜토리얼이 아닌 경우, 무조건 true 반환
    /// </summary>
    public bool IsPossibleToAction(QUEST_TYPE type)
    {
        if (!GameManager.Instance.IsTutorial) return true;

        bool isClear = _clearQuestDict[type];
        if (!isClear)
        {
            ToastManager.Instance.ShowToast("선행 퀘스트를 먼저 완료해주세요.");
        }

        return isClear;
    }

    /// <summary>
    /// 화살표 타겟 설정
    /// </summary>
    public void SetArrowTargetIndex(int index = -1)
    {
        if (!GameManager.Instance.IsTutorial || index >= _portals.Count || _curQuest.Type != QUEST_TYPE.MoveToMine) return;
        
        if (index == -1) 
        {
            _guideArrow.SettingTarget(null);
            return; 
        }

        _guideArrow.SettingTarget(_portals[index]);
    }
}
