using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractImage : MonoBehaviour
{
    [SerializeField] private Transform farCursor;
    [SerializeField] private Transform nearCursor;
    private Animator farAnim;
    private Animator nearAnim;

    private void Awake()
    {
        farAnim = Helper_Component.GetComponent<Animator>(farCursor);
        nearAnim = Helper_Component.GetComponent<Animator>(nearCursor);
        DontDestroyOnLoad(this);
    }


    public void SetNearCursor(int size)
    {
        farCursor.gameObject.SetActive(false);
        nearCursor.gameObject.SetActive(true);
        if (size == 1)
        {
            nearAnim.SetTrigger("IsSize_1");
        }
        else if (size == 3)
        {
            nearAnim.SetTrigger("IsSize_3");
        }
        else
            ToastManager.Instance.ShowToast("[System] 사이즈가 잘못되었습니다!");
    }
    public void SetFarCursor(int size)
    {
        farCursor.gameObject.SetActive(true);
        nearCursor.gameObject.SetActive(false);
        if (size == 1)
        {
            farAnim.SetTrigger("IsSize_1");
        }
        else if (size == 3)
        {
            farAnim.SetTrigger("IsSize_3");
        }
        else
            ToastManager.Instance.ShowToast("[System] 사이즈가 잘못되었습니다!");
    }
}
