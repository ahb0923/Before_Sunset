using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerInteractSensor : MonoBehaviour
{ 
    [SerializeField] private Collider2D _collider;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private LayerMask _layer;
    
    private BaseTower _tower;

    private HashSet<GameObject> _inside = new();
    
    private void Start()
    {
        _tower = Helper_Component.GetComponentInParent<BaseTower>(this.gameObject);
        ScanInitial();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Set alpha 0.3f");
            _sprite.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            if (_inside.Add(other.gameObject))
                SetSpriteAlpha(0.5f);
        }*/

        Debug.Log($"[Enter] {other.gameObject.name}");
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            if (_inside.Add(other.gameObject))
            {
                Debug.Log("Set to transparent");
                SetSpriteAlpha(0.5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            _inside.Remove(other.gameObject);
            if (_inside.Count == 0)
                SetSpriteAlpha(1f);
        }
    }

    private void SetSpriteAlpha(float alpha)
    {
        Color color = _sprite.color;
        color.a = alpha;
        _sprite.color = color;
    }

    private void ScanInitial()
    {
        ContactFilter2D filter = new()
        {
            useLayerMask = true,
            layerMask = _layer,
            useTriggers = true
        };

        List<Collider2D> results = new();
        _collider.OverlapCollider(filter, results);

        foreach (var col in results)
            _inside.Add(col.gameObject);

        SetSpriteAlpha(_inside.Count > 0 ? 0.5f : 1f);
    }

}
