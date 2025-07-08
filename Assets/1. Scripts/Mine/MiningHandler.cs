using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class MiningHandler : MonoBehaviour
{
    private PlayerStateHandler _playerState;
    private Collider2D _collider;

    [SerializeField] private bool isEntering = true;
    [SerializeField] private float stayTimeToTrigger = 1.5f;
    [SerializeField] private Image blackScreenImage;

    private Coroutine _triggerCoroutine;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerState = player.GetComponent<PlayerStateHandler>();

        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _triggerCoroutine == null)
        {
            var inputHandler = other.GetComponent<PlayerInputHandler>();
            if (inputHandler != null && inputHandler.IsRecallInProgress())
            {
                Debug.Log("귀환 중이므로 광산 입장/퇴장 불가");
                return;
            }

            _triggerCoroutine = StartCoroutine(WaitAndTrigger(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _triggerCoroutine != null)
        {
            StopCoroutine(_triggerCoroutine);
            _triggerCoroutine = null;
        }
    }

    private IEnumerator WaitAndTrigger(Collider2D player)
    {
        yield return new WaitForSeconds(stayTimeToTrigger);

        if (_playerState == null) yield break;

        yield return StartCoroutine(ScreenFadeController.Instance.FadeInOut(() =>
        {
            if (isEntering)
            {
                _playerState.EnterMiningArea();
                MapManager.Instance.MoveToRandomMap();
                Debug.Log("광산 입장");
            }
            else
            {
                _playerState.ExitMiningArea();
                MapManager.Instance.MoveToPreviousMap();
                Debug.Log("광산 퇴장");
            }
        }));

        _triggerCoroutine = null;
    }
}
