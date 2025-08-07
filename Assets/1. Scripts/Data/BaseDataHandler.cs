using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseDataHandler<TData> : IDataLoader where TData : class
{
    /// <summary>로컬 저장 파일 이름 (StreamingAssets 경로 기준)</summary>
    protected abstract string FileName { get; }

    /// <summary>데이터 ID 딕셔너리</summary>
    protected Dictionary<int, TData> dataIdDictionary = new();

    /// <summary>데이터 이름 딕셔너리 (대소문자 무시)</summary>
    protected Dictionary<string, TData> dataNameDictionary = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>ID로 데이터 가져오기</summary>
    public TData GetById(int id) => dataIdDictionary.TryGetValue(id, out var data) ? data : null;

    /// <summary>이름으로 데이터 가져오기</summary>
    public TData GetByName(string name) => dataNameDictionary.TryGetValue(name, out var data) ? data : null;

    /// <summary>모든 데이터 리스트 반환</summary>
    public List<TData> GetAllItems() => dataIdDictionary.Values.ToList();

    /// <summary>데이터에서 ID 추출</summary>
    protected abstract int GetId(TData data);

    /// <summary>데이터에서 Name 추출</summary>
    protected abstract string GetName(TData data);

    /// <summary>
    /// StreamingAssets에서 로컬 JSON 불러오기
    /// </summary>
    public async Task LoadAsyncLocal()
    {
        string path = Path.Combine(Application.streamingAssetsPath, FileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"[BaseDataHandler] 파일 없음: {path}");
            return;
        }

        string json = await File.ReadAllTextAsync(path);
        LoadFromJson(json);
    }

    /// <summary>
    /// Json 문자열을 데이터로 파싱하여 딕셔너리에 저장
    /// </summary>
    public void LoadFromJson(string json)
    {
        var list = JsonConvert.DeserializeObject<List<TData>>(json);
        dataIdDictionary.Clear();
        dataNameDictionary.Clear();

        foreach (var item in list)
        {
            int id = GetId(item);
            string name = GetName(item);

            dataIdDictionary[id] = item;
            if (!string.IsNullOrWhiteSpace(name))
                dataNameDictionary[name] = item;
        }

        AfterLoaded();
        Debug.Log($"[{typeof(TData).Name}Manager] 로드 완료: {dataIdDictionary.Count}개");
    }

    /// <summary>
    /// Firestore에서 컬렉션을 받아와 JSON으로 저장 (변경된 경우만)
    /// </summary>
    public async Task LoadFromFirestoreCollection(string collectionName, string documentIdField = "id")
    {
        var db = FirebaseFirestore.DefaultInstance;
        var snapshot = await db.Collection(collectionName).GetSnapshotAsync();

        List<TData> loadedList = new();

        foreach (var doc in snapshot.Documents)
        {
            var dict = doc.ToDictionary();

            // 문서 ID를 강제로 필드에 넣어주는 옵션
            if (!dict.ContainsKey(documentIdField) && int.TryParse(doc.Id, out int parsedId))
                dict[documentIdField] = parsedId;

            string json = JsonConvert.SerializeObject(dict);
            TData data = JsonConvert.DeserializeObject<TData>(json);

            if (data != null)
                loadedList.Add(data);
        }

        string jsonArray = JsonConvert.SerializeObject(loadedList, Formatting.Indented);
        string path = Path.Combine(Application.streamingAssetsPath, FileName);

        if (File.Exists(path))
        {
            string existing = File.ReadAllText(path);
            if (GetMd5Hash(existing) == GetMd5Hash(jsonArray))
            {
                Debug.Log($"[{typeof(TData).Name}] 변경 없음: 저장 생략");
                return;
            }
        }

        File.WriteAllText(path, jsonArray);
        Debug.Log($"[{typeof(TData).Name}] Firestore에서 받아 저장 완료 → {path}");
    }

    /// <summary>
    /// 해시 비교를 위한 MD5 계산기
    /// </summary>
    private string GetMd5Hash(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        return BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// 파싱 이후 추가 작업을 위해 오버라이드
    /// </summary>
    protected virtual void AfterLoaded() { }

    /// <summary>
    /// 전체 데이터 로그 출력
    /// </summary>
    public virtual void DebugLogAll(Func<TData, string> formatter = null)
    {
        foreach (var data in dataIdDictionary.Values)
        {
            string message = formatter != null ? formatter(data) : JsonConvert.SerializeObject(data);
            Debug.Log(message);
        }
    }

    // 향후 필요 시 구현
    public void SaveToJson() { }
}