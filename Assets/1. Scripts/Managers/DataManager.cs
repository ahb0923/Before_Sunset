using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
/*
    << 전체 ID >>
    ID 범위	        데이터 타입
    000 ~ 099	    광석 (Ore)
    100 ~ 199	    기본 자원 (Mineral)
    200 ~ 399	    희귀 자원 (Jewel)
    400 ~ 599	    타워 (Tower)
    600 ~ 699	    몬스터 (Monster)
    700 ~ 799	    장비 (Equipment)
    800 ~ 899	    스킬 (Skill)
    900 ~ 999	    구조물 (Structure)
    1000 ~ 1099	    미정

    << 타워 ID >>	
    ID 범위	        데이터 타입
    400 ~ 409	    구리 발사기
    410 ~ 419	    철 캐논
    420 ~ 429	    오팔 필드
    430 ~ 439	    다이아 프리즘
    440 ~ 449	    재생 타워
    450 ~ 459	    자력 타워
    460 ~ 469	    전깃줄 타워
	
    << 자원 ID >>	
    ID 범위	        데이터 타입
    100 ~ 109	    돌
    110 ~ 119	    구리
    120 ~ 129	    철
    130 ~ 139	    은
    140 ~ 149	    금
 */

public interface IDataLoader
{
    // protected abstract Task LoadAsyncWeb();
    Task LoadAsyncLocal();
    void LoadFromJson(string json);
}
public class DataManager : PlainSingleton<DataManager>
{
    // ↓ 인벤토리에 들어가지 않는 아이템
    public TowerDataHandler TowerData { get; private set; } = new();
    public MonsterDataHandler MonsterData { get; private set; } = new();
    // public OreDataHandler OreData { get; private set; } = new();



    // ↓ 인벤토리에 들어가는 아이템
    // public ItemDataHandler ItemData { get; private set; } = new();
    public MineralDataHandler MineralData { get; private set; } = new();
    // public JewelDataHandler JewelData { get; private set; } = new();
    // public EquipmentDataHandler EquipmentDataHandler { get; private set; } = new();


    public async Task InitAsync()
    {
        List<IDataLoader> loaders = new()
        {
            MineralData,
            TowerData,
            MonsterData,
            //JewelData,
            //OreData,
            //EquipmnetData,
        };

        foreach (var loader in loaders)
        {
            await loader.LoadAsyncLocal();
        }

        Debug.Log("[DataManager] 모든 데이터 초기화 완료");

        MonsterData.DebugLogAll();
    }
}
