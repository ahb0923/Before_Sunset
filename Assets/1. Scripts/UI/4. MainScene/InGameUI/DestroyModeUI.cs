using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyModeUI : MonoBehaviour
{
    [SerializeField] Transform onUI;
    private void Reset()
    {
        onUI = Helper_Component.FindChildByName(transform, "On");
    }
    private void Awake()
    {
        onUI.gameObject.SetActive(false);
    }
    public void SetMode(bool isDestroyOn)
    {
        if (isDestroyOn)
        {
            onUI.gameObject.SetActive(true);
        }
        else
        {
            onUI.gameObject.SetActive(false);
        }
    }
}
