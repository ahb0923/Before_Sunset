using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 벡터 값 저장을 위한 직렬화 벡터 구조체
/// </summary>
[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    // Vector3로의 암시적 변환
    public static implicit operator Vector3(SerializableVector3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }

    // SerializableVector3로의 암시적 변환
    public static implicit operator SerializableVector3(Vector3 vector)
    {
        return new SerializableVector3(vector);
    }
}

[Serializable]
public class GameData
{
    // 1. 플레이어 관련 : 위치 & 인벤토리
    public SerializableVector3 playerPosition;
    public InventorySaveData inventory;

    // 2. 디펜스 관련 : 코어 체력 & 타워 정보
    public int coreCurHp;
    public List<TowerSaveData> constructedTowers;
    public List<SmelterSaveData> constructedSmelters;

    // 3. 광산 관련 : 소환된 광산 정보 (광석 & 쥬얼도 포함)
    public MapLinkSaveData mapLinks;
    public List<MineSaveData> spawnedMines;

    // 4. 시간 관련 : 인게임 시간 정보 & 저장 버튼 누른 시간
    public TimeSaveData timeData;
    public string lastSaveDateTime;

    // 5. 업그레이드 관련 : 코어 & 플레이어 업그레이드 정보 / 샤드 개수
    public EssenceSaveData esenceSaveData;

    // 생성 시 기본적인 리스트만 초기화 (인벤토리 세이브 데이터도 안에는 리스트 초기화임)
    public GameData()
    {
        inventory = new InventorySaveData();
        constructedTowers = new List<TowerSaveData>();
        constructedSmelters = new List<SmelterSaveData>();
        mapLinks = new MapLinkSaveData();
        spawnedMines = new List<MineSaveData>();
        lastSaveDateTime = DateTime.UtcNow.ToString("o");
        esenceSaveData = new EssenceSaveData();
    }
}
