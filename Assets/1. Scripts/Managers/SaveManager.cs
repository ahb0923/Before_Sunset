using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface ISaveable
{
    void SaveData(GameData data);
}

public class SaveManager : MonoSingleton<SaveManager>
{
    private const string SAVE_KEY_OFFSET = "SaveSlot_";
    
    private HashSet<ISaveable> saveables = new HashSet<ISaveable>();

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 저장 데이터 인터페이스 오브젝트 추가
    /// </summary>
    public void AddSaveableObjects(ISaveable saveable)
    {
        saveables.Add(saveable);
    }

    /// <summary>
    /// 저장 데이터 인터페이스 오브젝트 해제
    /// </summary>
    public void ReleaseSaveableObjects(ISaveable saveable)
    {
        if(saveables.Contains(saveable))
            saveables.Remove(saveable);
    }

    /// <summary>
    /// 게임 데이터 저장 경로 반환
    /// </summary>
    private string GetFilePathForSlot(int slotIndex)
    {
        string fileName;

        string playerPrefsKey = SAVE_KEY_OFFSET + slotIndex;
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            fileName = PlayerPrefs.GetString(playerPrefsKey);
            Debug.Log($"저장 슬롯 {slotIndex}번 - 파일 덮어씌움 : {fileName}");
            return Path.Combine(Application.persistentDataPath, fileName);
        }
        else
        {
            fileName = $"GameData_{slotIndex}.json";
            PlayerPrefs.SetString(playerPrefsKey, fileName);
            PlayerPrefs.Save();
            Debug.Log($"저장 슬롯 {slotIndex}번 - 새 파일 저장 : {fileName}");
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }

    /// <summary>
    /// 저장 슬롯에 게임 데이터 저장
    /// </summary>
    public void SaveGameToSlot(int slotIndex)
    {
        GameData data = new GameData();
        // 이곳에서 ISaveable 인터페이스를 가진 애들을 돌면서 저장 진행

        string path = GetFilePathForSlot(slotIndex);

        string jsonData = JsonUtility.ToJson(data, true);
        if(jsonData.Length == 0)
        {
            Debug.LogWarning("[SaveManager] Json 데이터가 비어 있습니다.");
            return;
        }

        File.WriteAllText(path, jsonData);
        Debug.Log($"{slotIndex}번 슬롯에 게임 저장 완료");
    }

    /// <summary>
    /// 저장 슬롯에서 게임 데이터 로드
    /// </summary>
    public GameData LoadGameFromSlot(int slotIndex)
    {
        string path = GetFilePathForSlot(slotIndex);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveManager] {slotIndex}번 슬롯에 저장된 게임 파일이 없습니다.");
            return null;
        }

        string jsonData = File.ReadAllText(path);
        GameData data = JsonUtility.FromJson<GameData>(jsonData);
        Debug.Log($"{slotIndex}번 슬롯에서 게임 불러오기 완료");
        return data;
    }

    /// <summary>
    /// 특정 저장 슬롯의 게임 데이터 삭제
    /// </summary>
    public void DeleteSaveFileFromSlot(int slotIndex)
    {
        string path = GetFilePathForSlot(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"{slotIndex}번 슬롯에 저장된 파일 삭제 완료");
        }

        // PlayerPrefs에서도 해당 슬롯의 파일 이름 정보를 삭제
        string playerPrefsKey = SAVE_KEY_OFFSET + slotIndex;
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            PlayerPrefs.DeleteKey(playerPrefsKey);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 특정 저장 슬롯의 게임 데이터 유무 확인
    /// </summary>
    public bool DoesSaveSlotExist(int slotIndex)
    {
        // PlayerPrefs에 기록이 없으면 파일도 없는 것으로 간주
        string playerPrefsKey = SAVE_KEY_OFFSET + slotIndex;
        if (!PlayerPrefs.HasKey(playerPrefsKey))
        {
            return false;
        }

        string fileName = PlayerPrefs.GetString(playerPrefsKey);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        return File.Exists(path);
    }
}
