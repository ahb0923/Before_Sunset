using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    [SerializeField] private BaseTower _tower;

    public Canvas canvas;
    public Image hpBar;

    public void Init()
    {
        if (canvas == null)
            Debug.LogWarning("Canvas attach 누락");
        if (hpBar == null)
            Debug.LogWarning("HpBar attach 누락");

        _tower.statHandler.OnHpChanged += UpdateHpBar;
    }
    private void UpdateHpBar(float curr, float max)
    {
        hpBar.fillAmount = curr / max;
    }
}
