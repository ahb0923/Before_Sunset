using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class TowerDataManager : IDataLoader
{
    private const string TowerDataURL = "https://script.google.com/macros/s/your_url_here/exec";

    private Dictionary<int, TowerData> _towerDict = new();

    public TowerData Get(int id) => _towerDict.TryGetValue(id, out var data) ? data : null;

    public async Task LoadAsync()
    {
        string json = await JsonDownloader.DownloadJson(TowerDataURL);
        if (!string.IsNullOrEmpty(json))
        {
            LoadFromJson(json);
        }
        else
        {
            Debug.LogError("[TowerDataManager] JSON 로딩 실패");
        }
    }

    public void LoadFromJson(string json)
    {
        var towers = JsonHelper.FromJson<TowerData>(json);
        _towerDict.Clear();
        foreach (var tower in towers)
            _towerDict[tower.id] = tower;

        Debug.Log($"[TowerDataManager] 타워 {towers.Length}개 로드됨");
    }
}
