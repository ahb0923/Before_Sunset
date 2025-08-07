using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDataHandler
{
    private List<ItemDatabase> _allItems = new(); 
    public IReadOnlyList<ItemDatabase> AllItems => _allItems;

    // 이름 검색용
    private Dictionary<int, ItemDatabase> _byId = new();
    // ID 검색용(대소문자 구분x, but 기왕이면 지킬것)
    private Dictionary<string, ItemDatabase> _byName = new(StringComparer.OrdinalIgnoreCase);
    // Type 검색용(Mineral, Jewel, Equipment)
    private Dictionary<Type, List<ItemDatabase>> _byType = new();


    // 데이터 추가되면 확장성있게 구조 변경 필요 일단은 하드코딩 => DataManager에서 Init 호출
    public void Init(MineralDataHandler mineral, JewelDataHandler jewel, EquipmentDataHandler equipment)
    {
        _allItems.Clear();
        _byId.Clear();
        _byName.Clear();
        _byType.Clear();

        foreach (var item in mineral.GetAllItems()) 
            Register(item);
        foreach (var item in jewel.GetAllItems()) 
            Register(item);
        foreach (var item in equipment.GetAllItems()) 
            Register(item);
    }

    private void Register(ItemDatabase item)
    {
        _allItems.Add(item);

        _byId[item.id] = item;

        if (!string.IsNullOrWhiteSpace(item.itemName))
            _byName[item.itemName] = item;

        Type classType = item.GetType();
        if (!_byType.TryGetValue(classType, out var list))
            _byType[classType] = list = new();
        list.Add(item);
    }

    /// <summary>
    /// ID로 아이템 데이터 조회
    /// </summary>
    public ItemDatabase GetById(int id) =>
        _byId.TryGetValue(id, out var item) ? item : null;

    /// <summary>
    /// 이름으로 아이템 데이터 조회
    /// </summary>
    public ItemDatabase GetByName(string name) =>
        _byName.TryGetValue(name, out var item) ? item : null;

    /// <summary>
    /// 이름으로 아이템 ID 조회 (실패 시 -1)
    /// </summary>
    public int GetIdByName(string name) =>
        _byName.TryGetValue(name, out var item) ? item.id : -1;

    /// <summary>
    /// 타입별 아이템 데이터 조회
    /// </summary>
    public IEnumerable<T> GetByType<T>() where T : ItemDatabase =>
        _byType.TryGetValue(typeof(T), out var list) ? list.Cast<T>() : Enumerable.Empty<T>();
}
