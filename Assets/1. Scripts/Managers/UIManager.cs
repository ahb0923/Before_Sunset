using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    public GameObject testBuildSlot;

    private void Update()
    {

        // 테스트용 E키
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (testBuildSlot != null)
            {
                bool isActive = testBuildSlot.activeSelf;
                testBuildSlot.SetActive(!isActive);
            }
        }
    }
}
