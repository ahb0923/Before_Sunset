using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Smelter : MonoBehaviour, IPointerClickHandler
{
    public SmelterDatabase smelterData;
    
    public Item InputItem { get; private set; }
    public Item OutputItem { get; private set; }
    
    private Coroutine _smeltCoroutine;
    public bool isSmelting;
    public event Action<float> OnSmeltingProgress;

    private void Awake()
    {
        // 테스트용 코드
        smelterData = DataManager.Instance.SmelterData.GetById(900);
    }

    /// <summary>
    /// 제련 시도 메서드
    /// </summary>
    public void TrySmelt()
    {
        if (_smeltCoroutine != null)
        {
            return;
        }
        
        if (!isSmelting)
        {
            StartSmelting();
        }
    }

    /// <summary>
    /// 제련 시작 메서드
    /// </summary>
    private void StartSmelting()
    {
        if (InputItem == null)
        {
            return;
        }
        
        var mineralData = DataManager.Instance.MineralData.GetById(InputItem.Data.id);
        
        if (OutputItem == null || mineralData.ingotId == OutputItem.Data.id)
        {
            _smeltCoroutine = StartCoroutine(C_Smelt(mineralData));
        }
        else
        {
            isSmelting = false;
        }
    }
    
    /// <summary>
    /// 제련 메서드
    /// </summary>
    /// <param name="mineralData"></param>
    /// <returns></returns>
    private IEnumerator C_Smelt(MineralDatabase mineralData)
    {
        isSmelting = true;
        InputItem.stack--;

        if (InputItem.stack <= 0)
        {
            InputItem = null;
        }
        
        RefreshUI();
        
        float duration = mineralData.smeltingTime;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            OnSmeltingProgress?.Invoke(progress);

            yield return null;
        }
        
        if (OutputItem == null)
        {
            OutputItem = new Item(DataManager.Instance.ItemData.GetId(mineralData.ingotId));
        }
        
        OutputItem.stack++;
        _smeltCoroutine = null;
        OnSmeltingProgress?.Invoke(0);
        RefreshUI();

        if (InputItem != null)
        {
            StartSmelting();
        }
        else
        {
            isSmelting = false;
        }
    }

    /// <summary>
    /// 제련 후 UI 새로고침할때 사용되는 메서드
    /// </summary>
    private void RefreshUI()
    {
        UIManager.Instance.SmelterUI.RefreshSlots();
    }

    /// <summary>
    /// 슬롯에서 제련소 아이템을 바꿔줄때 사용될 메서드
    /// </summary>
    /// <param name="item"></param>
    public void SetInputItem(Item item)
    {
        InputItem = item;
        TrySmelt();
    }

    /// <summary>
    /// 슬롯에서 제련소 아이템을 바꿔줄때 사용될 메서드
    /// </summary>
    /// <param name="item"></param>
    public void SetOutputItem(Item item)
    {
        OutputItem = item;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.SmelterUI.OpenSmelter(this);
    }
}