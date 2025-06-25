using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseDataHandler<TData> : IDataLoader where TData : class
{
    // 구글 스프레드시트에서 배포 된 url 주소  =>  Local에 JSON 파일로 저장 시 필요 없음
    //protected abstract string DataUrl { get; }

    // 로컬 시 로드할 파일 이름
    protected abstract string FileName { get; }

    // 아이템 ID가 key값
    protected Dictionary<int, TData> dataIdDictionary = new();

    // 아이템 Name이 key값    StringComparer.OrdinalIgnoreCase => 대소문자 상관 x  sToNE
    protected Dictionary<string, TData> dataNameDictionary = new(StringComparer.OrdinalIgnoreCase);

    // ID로 데이터 찾기
    public TData GetById(int id) => dataIdDictionary.TryGetValue(id, out var data) ? data : null;

    // Name으로 데이터 찾기    DataManager.Instance.TowerData.GeyByName("sTonNe");
    public TData GetByName(string name) => dataNameDictionary.TryGetValue(name, out var data) ? data : null;


    // web에서 json데이터 받을 때 사용   =>   Local에 정보 있다면 필요없음   
    /*public async Task LoadAsyncWeb()
    {
        string json = await JsonDownloader.DownloadJson(DataUrl);
        if (!string.IsNullOrEmpty(json))
        {
            LoadFromJson(json);
        }
        else
        {
            Debug.LogError($"[{typeof(TData).Name}Manager] JSON 로딩 실패");
        }
    }
    */
    public async Task LoadAsyncLocal()
    {
        string path = Path.Combine(Application.streamingAssetsPath, FileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"[BaseDataManager] 파일 없음: {path}");
            return;
        }

        string json = await File.ReadAllTextAsync(path);
        LoadFromJson(json);
    }

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

        Debug.Log($"[{typeof(TData).Name}Manager] 로드 완료: {dataIdDictionary.Count}개");
        DebugLogAll();
    }

    public void SaveToJson()
    {

    }


    public virtual void DebugLogAll(Func<TData, string> formatter = null)
    {
        foreach (var data in dataIdDictionary.Values)
        {
            string message = formatter != null ? formatter(data) : JsonConvert.SerializeObject(data);
            Debug.Log(message);
        }
    }

    protected abstract int GetId(TData data);
    protected abstract string GetName(TData data);

}
