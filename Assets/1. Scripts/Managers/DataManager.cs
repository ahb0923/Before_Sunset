using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public interface IDataLoader
{
    // protected abstract Task LoadAsyncWeb();
    Task LoadAsyncLocal();
    void LoadFromJson(string json);
}

public class DataManager : PlainSingleton<DataManager>
{
    public MineralDataHandler MineralData { get; private set; } = new();
    public TowerDataHandler TowerData { get; private set; } = new();
    public MonsterDataHandler MonsterData { get; private set; } = new();

    public async Task InitAsync()
    {
        List<IDataLoader> loaders = new()
        {
            MineralData,
            TowerData,
            MonsterData
        };

        foreach (var loader in loaders)
        {
            await loader.LoadAsyncLocal();
        }

        Debug.Log("[DataManager] 모든 데이터 초기화 완료");

        MonsterData.DebugLogAll();
    }
}
