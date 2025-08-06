using System.Collections;
using UnityEngine;

public class TutorialPortal : MonoBehaviour
{
    [SerializeField] private Transform _moveTransform;
    [SerializeField] private TutorialSpawn _spawner;

    private IEnumerator C_MovePlayer(BasePlayer player)
    {
        QuestManager.Instance?.Arrow?.SettingTarget(null);

        yield return StartCoroutine(ScreenFadeController.Instance.FadeInOut(() =>
        {
            player.SetPlayerInBase(false);
            _spawner.SpawnOres();

            player.transform.position = _moveTransform.position;
        }));

        QuestManager.Instance?.AddQuestAmount(QUEST_TYPE.MoveToMine);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<BasePlayer>(out var player))
        {
            StartCoroutine(C_MovePlayer(player));
        }
    }
}
