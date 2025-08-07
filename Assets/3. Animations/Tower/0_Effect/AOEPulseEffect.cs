using System.Collections;
using UnityEngine;

public class AOEPulseEffect : MonoBehaviour, IPoolable
{
    [SerializeField] private Material pulseMaterial;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float thickness = 0.5f;
    [SerializeField] public Color color = Color.white;
    private int _id = 10001;
    private bool _isReverse = false;

    private Material _runtimeMaterial;
    private SpriteRenderer _sprite;

    private void Awake()
    {
        // 머티리얼 인스턴스화 (다른 오브젝트와 공유 방지)
        _runtimeMaterial = new Material(pulseMaterial);
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.material = _runtimeMaterial;
    }

    public void Play()
    {
        StartCoroutine(C_Pulse());
    }

    private IEnumerator C_Pulse()
    {
        _runtimeMaterial.SetColor("_Color", color);
        _runtimeMaterial.SetFloat("_Thickness", thickness);

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;
            if (_isReverse) 
                progress = 1 - (time / duration);
            _runtimeMaterial.SetFloat("_Progress", progress);
            yield return null;
        }

        // 종료 후 리셋 또는 오브젝트 비활성화
        _runtimeMaterial.SetFloat("_Progress", 0f);
        PoolManager.Instance.ReturnToPool(_id, gameObject);
    }

    public void SetReverse()
    {
        _isReverse = true;
    }

    public void SetSize(float size)
    {
        size += 0.5f;
        gameObject.transform.localScale = new Vector2 (size, size);
    }

    public int GetId()
    {
        return _id;
    }

    public void OnInstantiate()
    {
    }

    public void OnGetFromPool()
    {
    }

    public void OnReturnToPool()
    {
        _isReverse=false;
    }
}