using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//Data ID 링크 : https://docs.google.com/spreadsheets/d/1n4f79j8bouq0bDa4URmX-OY1y2ZLqQ7RPAfw86tyr0I/edit?gid=0#gid=0

public interface IDataLoader
{
    // protected abstract Task LoadAsyncWeb();
    Task LoadAsyncLocal();
    void LoadFromJson(string json);
}
public class DataManager : PlainSingleton<DataManager>
{
    private Task _initTask;
    private bool _isInitialized;

    // ↓ 인벤토리에 들어가지 않는 아이템(오브젝트 데이터)
    public TowerDataHandler TowerData { get; private set; } = new();
    public MonsterDataHandler MonsterData { get; private set; } = new();
    public OreDataHandler OreData { get; private set; } = new();
    public ProjectileDataHandler ProjectileData { get; private set; } = new();
    public SmelterDataHandler SmelterData { get; private set; } = new();
    public DebuffDataHandler DebuffData { get; private set; } = new();


    // ↓ 인벤토리에 들어가는 아이템 데이터
    public ItemDataHandler ItemData { get; private set; } = new();
    public MineralDataHandler MineralData { get; private set; } = new();
    public JewelDataHandler JewelData { get; private set; } = new();
    public EquipmentDataHandler EquipmentData { get; private set; } = new();

    // ↓ 시스템 데이터
    public WaveDataHandler WaveData { get; private set; } = new();
    public ClearRewardDataHandler ClearRewardData { get; private set; } = new();
    public MonsterRewardDataHandler MonsterRewardData { get; private set; } = new();
    public MapDataHandler MapData { get; private set; } = new();
    public UpgradeDataHandler UpgradeData { get; private set; } = new();




    public Task InitCheck()
    {
        if (_initTask != null)
            return _initTask;

        _initTask = InitAsync();
        return _initTask;
    }

    public async Task InitAsync()
    {
        if (_isInitialized)
            return;

        // Firestore에서 고정 데이터 받아와서 저장
        await TowerData.LoadFromFirestoreCollection("TowerData");
        await MonsterData.LoadFromFirestoreCollection("MonsterData");
        await SmelterData.LoadFromFirestoreCollection("SmelterData");
        await ProjectileData.LoadFromFirestoreCollection("ProjectileData");
        await OreData.LoadFromFirestoreCollection("OreData");
        await MineralData.LoadFromFirestoreCollection("MineralData");
        await JewelData.LoadFromFirestoreCollection("JewelData");
        await EquipmentData.LoadFromFirestoreCollection("EquipmentData");
        await DebuffData.LoadFromFirestoreCollection("DebuffData");

        await WaveData.LoadFromFirestoreCollection("WaveData");
        await ClearRewardData.LoadFromFirestoreCollection("ClearRewardData");
        await MonsterRewardData.LoadFromFirestoreCollection("MonsterRewardData");
        await MapData.LoadFromFirestoreCollection("MapData");
        await UpgradeData.LoadFromFirestoreCollection("UpgradeData");


        List<IDataLoader> loaders = new()
        {
            TowerData,
            MonsterData,
            SmelterData,
            ProjectileData,
            OreData,
            MineralData,
            JewelData,
            EquipmentData,
            DebuffData,
            WaveData,
            ClearRewardData,
            MonsterRewardData,
            MapData,
            UpgradeData
        };

        foreach (var loader in loaders)
        {
            await loader.LoadAsyncLocal();
        }

        ItemData.Init(MineralData, JewelData, EquipmentData);


        Debug.Log("[DataManager] 모든 데이터 초기화 완료");
        _isInitialized = true;
    }
}
