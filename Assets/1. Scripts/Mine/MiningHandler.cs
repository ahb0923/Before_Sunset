using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class MiningHandler : MonoBehaviour
{
    public enum PortalDirection { North, East, South, West }

    [SerializeField] private PortalDirection portalDirection;
    [SerializeField] private bool isEntering;  // 입장/퇴장 자동 결정
    [SerializeField] private float stayTimeToTrigger = 0.5f;

    private BasePlayer _player;
    private Coroutine _triggerCoroutine;
    public PortalDirection CurrentPortalDirection => portalDirection;


    private void Start()
    {
        UpdateEnteringState();
    }

    private void UpdateEnteringState()
    {
        if (MapManager.Instance.CurrentMapIndex == 0)
        {
            isEntering = true;
        }
        else
        {
            var lastDir = MapManager.Instance.LastEnteredPortalDirection ?? PortalDirection.North;
            var oppositeDir = GetOppositeDirection(lastDir);

            isEntering = portalDirection != oppositeDir;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (_player == null)
        {
            _player = other.GetComponentInChildren<BasePlayer>();
            if (_player == null)
            {
                Debug.LogWarning("PlayerStateHandler를 찾지 못했습니다.");
                return;
            }
        }

        //var inputHandler = other.GetComponentInChildren<PlayerInputHandler>();
        //if (inputHandler != null)
        //{
        //    Debug.Log("귀환 중이므로 광산 입장/퇴장 불가");
        //    return;
        //}

        if (_triggerCoroutine == null)
            _triggerCoroutine = StartCoroutine(WaitAndTrigger());
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

        if (_player == null) yield break;

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
                _player.SetPlayerInBase(true);
            }
            else
            {
                _player.SetPlayerInBase(false);
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