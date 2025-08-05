using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputGameObject
{
    public GameObject pressedOff;
    public GameObject pressedOn;

    public void SetPressed(bool isPress)
    {
        pressedOff.SetActive(!isPress);
        pressedOn.SetActive(isPress);
    }
}

public class InputGuideEffect : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private List<InputGameObject> _inputGameObjects;
    [SerializeField] private float _duration;
    [SerializeField] private bool _isToggle;

    [SerializeField] private float _oncePressedTime = 0.3f;

    private float _timer = 0f;
    private int _index = 0;

    private void Start()
    {
        UpdateGuideAnimation();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > _duration)
        {
            _timer = 0f;
            UpdateGuideAnimation();
        }
    }

    private void UpdateGuideAnimation()
    {
        _inputGameObjects[_index].SetPressed(false);

        _index = _index + 1 >= _inputGameObjects.Count ? 0 : _index + 1;

        _inputGameObjects[_index].SetPressed(true);
        _animator.SetTrigger("Trigger");
        if (!_isToggle)
            StartCoroutine(C_PressedOnce());
    }

    private IEnumerator C_PressedOnce()
    {
        yield return Helper_Coroutine.WaitSeconds(_oncePressedTime);

        _inputGameObjects[_index].SetPressed(false);
    }
}
