using System.Collections;
using UnityEngine;

public class TutorialPortal : MonoBehaviour
{
    [SerializeField] private int _portalIndex = 0;
    [SerializeField] private Transform _moveTransform;

    private IEnumerator C_MovePlayer(BasePlayer player)
    {
        yield return StartCoroutine(ScreenFadeController.Instance.FadeInOut(() =>
        {
            QuestManager.Instance.AddQuestAmount(QUEST_TYPE.MoveToMine, _portalIndex);
            QuestManager.Instance.SetArrowTargetIndex(_portalIndex);
            player.SetPlayerInBase(_portalIndex == 0);
            player.transform.position = _moveTransform.position;
        }));

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<BasePlayer>(out var player))
        {
            StartCoroutine(C_MovePlayer(player));
        }
    }
}
