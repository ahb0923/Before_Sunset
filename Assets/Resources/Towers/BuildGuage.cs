using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildGuage : MonoBehaviour,IPoolable
{
    private int id = 10002;

    [SerializeField] private Animator animator;
    private void Awake()
    {
        animator = Helper_Component.GetComponent<Animator>(gameObject);
    }

    public int GetId()
    {
        return id;
    }

    public void OnGetFromPool()
    {
        animator.Play("BuildGuage_Running");
    }

    public void OnInstantiate()
    {
    }

    public void OnReturnToPool()
    {
    }
}
