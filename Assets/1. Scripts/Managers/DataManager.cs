using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public interface IDataLoader
{
    Task LoadAsync();
    public void LoadFromJson(string json);
}

public class DataManager : PlainSingleton<DataManager>
{
    public MineralDataManager ItemData { get; private set; } = new();
    public TowerDataManager TowerData { get; private set; } = new();
    public MonsterDataManager MonsterData { get; private set; } = new();

    public async Task InitAsync()
    {
        List<IDataLoader> loaders = new() { ItemData }; //, TowerData, MonsterData 

        foreach (var loader in loaders)
        {
            await loader.LoadAsync();
        }

        Debug.Log("[DataManager] 모든 데이터 초기화 완료");
    }
}
