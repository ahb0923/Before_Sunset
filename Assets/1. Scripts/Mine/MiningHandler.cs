using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class MiningHandler : MonoBehaviour
{
    public enum PortalDirection { North, East, South, West }

    [SerializeField] private PortalDirection portalDirection;
    [SerializeField] private bool isEntering;  // 입장/퇴장 자동 결정
    [SerializeField] private float stayTimeToTrigger = 1.5f;

    private PlayerStateHandler _playerState;
    private Coroutine _triggerCoroutine;
    public PortalDirection CurrentPortalDirection => portalDirection;


    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerState = player.GetComponent<PlayerStateHandler>();

        // portalDirection 기반 isEntering 결정은 여기서 하자
        UpdateEnteringState();
    }

    private void UpdateEnteringState()
    {
        if (MapManager.Instance.CurrentMapIndex == 0)
        {
            isEntering = true;
            Debug.Log($"[MiningHandler] 기본맵: 모든 포탈 입장용 (isEntering={isEntering}), portalDirection={portalDirection}");
        }
        else
        {
            var lastDir = MapManager.Instance.LastEnteredPortalDirection ?? PortalDirection.North;
            var oppositeDir = GetOppositeDirection(lastDir);

            isEntering = portalDirection != oppositeDir;
            Debug.Log($"[MiningHandler] 광산맵: lastEnteredDir={lastDir}, oppositeDir={oppositeDir}, portalDirection={portalDirection}, isEntering={isEntering}");
        }
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

            _triggerCoroutine = StartCoroutine(WaitAndTrigger());
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

    private IEnumerator WaitAndTrigger()
    {
        yield return new WaitForSeconds(stayTimeToTrigger);

        if (_playerState == null) yield break;

        yield return StartCoroutine(ScreenFadeController.Instance.FadeInOut(() =>
        {
            if (isEntering)
            {
                MapManager.Instance.MoveToMapByDirection(portalDirection);
            }
            else
            {
                MapManager.Instance.MoveToPreviousMap();
            }

            bool isBaseMapAfterMove = MapManager.Instance.CurrentMapIndex == 0;

            if (isBaseMapAfterMove)
            {
                _playerState.ExitMiningArea();
            }
            else
            {
                _playerState.EnterMiningArea();
            }
        }));


        _triggerCoroutine = null;
    }

    private PortalDirection GetOppositeDirection(PortalDirection dir)
    {
        return dir switch
        {
            PortalDirection.North => PortalDirection.South,
            PortalDirection.South => PortalDirection.North,
            PortalDirection.East => PortalDirection.West,
            PortalDirection.West => PortalDirection.East,
            _ => dir,
        };
    }
}