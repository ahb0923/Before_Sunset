using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    [Header("맵 관련 설정")]
    [SerializeField] private GameObject[] mapChunks;
    [SerializeField] private Transform player;

    private int currentMapIndex = 0;
    private Stack<int> mapHistory = new Stack<int>();

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < mapChunks.Length; i++)
        {
            mapChunks[i].SetActive(i == 0);
        }

        currentMapIndex = 0;
        mapHistory.Clear();
    }

    public void MoveToRandomMap()
    {
        if (mapChunks.Length <= 1) return;

        int nextIndex;
        do
        {
            nextIndex = Random.Range(1, mapChunks.Length);
        } while (nextIndex == currentMapIndex);

        MoveToMap(nextIndex);
    }

    public void MoveToPreviousMap()
    {
        if (mapHistory.Count == 0) return;

        int previousIndex = mapHistory.Pop();
        MoveToMap(previousIndex, false);
    }

    public void ReturnToHomeMap()
    {
        if (currentMapIndex == 0) return;

        mapHistory.Clear();
        MoveToMap(0, false);
    }

    private void MoveToMap(int targetIndex, bool addToHistory = true)
    {
        if (targetIndex < 0 || targetIndex >= mapChunks.Length) return;

        mapChunks[currentMapIndex].SetActive(false);
        mapChunks[targetIndex].SetActive(true);

        if (addToHistory)
        {
            mapHistory.Push(currentMapIndex);
        }

        currentMapIndex = targetIndex;
        player.position = mapChunks[targetIndex].transform.position;
    }
}
