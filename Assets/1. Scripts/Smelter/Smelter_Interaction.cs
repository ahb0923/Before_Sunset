using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Smelter_Interaction : MonoBehaviour, IPointerClickHandler
{
    private Smelter _smelter;

    public void Init(Smelter smelter)
    {
        _smelter = smelter;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.SmelterUI.OpenSmelter(_smelter);
    }
}
