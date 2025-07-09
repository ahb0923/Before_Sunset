using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraHandler : MonoBehaviour
{
    public static AuraHandler Instance;

    [SerializeField] private GameObject auraPrefab;
    private GameObject activeAura;

    private void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        if (activeAura == null)
        {
            activeAura = Instantiate(auraPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    public void Hide()
    {
        if (activeAura != null)
        {
            Destroy(activeAura);
        }
    }
}
