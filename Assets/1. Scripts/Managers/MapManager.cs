using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoSingleton<MapManager>
{
    // ========== [맵 관련 필드] ==========
    [Header("맵 관련 설정")]
    [SerializeField] private GameObject[] mapChunks;
    [SerializeField] private Transform player;

    private int currentMapIndex = 0;
    private int previousMapIndex = -1;

    protected override void Awake()
    {
        base.Awake();

        // 모든 맵 중 기본 맵(0번)만 활성화
        for (int i = 0; i < mapChunks.Length; i++)
        {
            mapChunks[i].SetActive(i == 0);
        }

        currentMapIndex = 0;
        previousMapIndex = -1;
    }

    // ========== [맵 전환 메서드] ==========
    public void MoveToRandomMap()
    {
        int nextIndex;

        if (mapChunks.Length <= 1) return;

        do
        {
            nextIndex = Random.Range(1, mapChunks.Length);
        } while (nextIndex == currentMapIndex);

        MoveToMap(nextIndex);
    }

    public void MoveToPreviousMap()
    {
        if (previousMapIndex == -1) return;

        MoveToMap(previousMapIndex);
    }

    private void MoveToMap(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= mapChunks.Length) return;

        mapChunks[currentMapIndex].SetActive(false);

        mapChunks[targetIndex].SetActive(true);

        previousMapIndex = currentMapIndex;
        currentMapIndex = targetIndex;

        player.position = mapChunks[targetIndex].transform.position + new Vector3(0f, 0f, 0f);
    }
}
