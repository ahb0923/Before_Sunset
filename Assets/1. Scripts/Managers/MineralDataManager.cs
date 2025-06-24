using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class MineralDataManager : IDataLoader
{
    private const string ItemDataURL = "https://script.google.com/macros/s/AKfycbz0b4dwz-nu3icZa1vauBU0EWtUa8v259evQF4EJ_MWIkLiYZHvK0LbItdWmQ3gFdcb/exec";

    private Dictionary<int, MineralData> _itemDict = new();

    public MineralData Get(int id) => _itemDict.TryGetValue(id, out var data) ? data : null;

    public async Task LoadAsync()
    {
        string json = await JsonDownloader.DownloadJson(ItemDataURL);
        if (!string.IsNullOrEmpty(json))
        {
            LoadFromJson(json);
        }
        else
        {
            Debug.LogError("[ItemManager] ItemData 로딩 실패");
        }
    }

    private async Task<string> ReadFileAsync(string path)
    {
        using StreamReader reader = new(path);
        return await reader.ReadToEndAsync();

        // webGL 빌드 시 필요
        /*
#if UNITY_WEBGL && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(path);
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError($"[ItemDataManager] Failed to read JSON: {www.error}");

        return www.downloadHandler.text;
#else
        // 이 부분에 상기의 코드 넣을것
#endif*/
    }
    public void LoadFromJson(string json)
    {
        var items = JsonHelper.FromJson<MineralData>(json);
        _itemDict.Clear();

        foreach (var item in items)
        {
            _itemDict[item.id] = item;
        }

        Debug.Log($"[ItemDataManager] 로드 완료: {_itemDict.Count}개 아이템");
        DebugLogAllItems();
    }

    public void DebugLogAllItems()
    {
        if (_itemDict == null || _itemDict.Count == 0)
        {
            Debug.LogWarning("[ItemDataManager] 아이템 데이터가 비어 있습니다.");
            return;
        }

        Debug.Log($"[ItemDataManager] 전체 아이템 목록 ({_itemDict.Count}개):");

        foreach (var item in _itemDict.Values)
        {
            Debug.Log($"ID: {item.id}, Name: {item.itemName}, Text: {item.context}");
        }
    }
}
