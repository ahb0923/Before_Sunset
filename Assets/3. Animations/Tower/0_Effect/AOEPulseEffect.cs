using System.Collections;
using UnityEngine;

public class AOEPulseEffect : MonoBehaviour, IPoolable
{
    [SerializeField] private Material pulseMaterial;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float thickness = 0.2f;
    [SerializeField] public Color color = Color.white;
    private int id = 10001;
    private bool isReverse = false;

    private Material runtimeMaterial;

    private void Awake()
    {
        // 머티리얼 인스턴스화 (다른 오브젝트와 공유 방지)
        runtimeMaterial = new Material(pulseMaterial);
        GetComponent<SpriteRenderer>().material = runtimeMaterial;
    }

    public void Play()
    {
        StartCoroutine(C_Pulse());
    }

    private IEnumerator C_Pulse()
    {
        runtimeMaterial.SetColor("_Color", color);
        runtimeMaterial.SetFloat("_Thickness", thickness);

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;
            if (isReverse) 
                progress = 1 - (time / duration);
            runtimeMaterial.SetFloat("_Progress", progress);
            yield return null;
        }

        // 종료 후 리셋 또는 오브젝트 비활성화
        runtimeMaterial.SetFloat("_Progress", 0f);
        PoolManager.Instance.ReturnToPool(id, gameObject);
    }

    public void SetReverse()
    {
        isReverse = true;
    }

    public int GetId()
    {
        return id;
    }

    public void OnInstantiate()
    {
    }

    public void OnGetFromPool()
    {
    }

    public void OnReturnToPool()
    {
        isReverse=false;
    }
}