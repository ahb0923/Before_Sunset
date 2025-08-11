using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISaveable
{
    void SaveData(GameData data);
    void LoadData(GameData data);
}

public class SaveManager : MonoSingleton<SaveManager>
{
    private const string SAVE_KEY_OFFSET = "SaveSlot_";

    private HashSet<ISaveable> saveables = new HashSet<ISaveable>();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // 시작 화면은 로드 X
        if (SceneManager.GetActiveScene().buildIndex == 0) return;

        if (GlobalState.Index == 1 || GlobalState.Index == 2 || GlobalState.Index == 3 || GlobalState.Index == 99)
        {
            LoadGameFromGlobalIndex(GlobalState.Index);
        }
    }

    /// <summary>
    /// 모든 세이브 가능 인스턴스를 리스트에 저장
    /// </summary>
    private void UpdateSavebles()
    {
        saveables.Clear();

        foreach(ISaveable saveable in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISaveable>())
        {
            saveables.Add(saveable);
        }
        saveables.Add(InventoryManager.Instance.Inventory);
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
            return Path.Combine(Application.persistentDataPath, fileName);
        }
        else
        {
            fileName = $"GameData_{slotIndex}.json";
            PlayerPrefs.SetString(playerPrefsKey, fileName);
            PlayerPrefs.Save();
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }

    /// <summary>
    /// 저장 슬롯에 게임 저장
    /// </summary>
    public void SaveGameToSlot(int slotIndex = 99)
    {
        UpdateSavebles();

        // 게임 내 데이터 저장
        GameData data = new GameData();
        foreach(ISaveable saveable in saveables)
        {
            saveable.SaveData(data);
        }

        // 경로 가져오기 or 생성
        string path = GetFilePathForSlot(slotIndex);

        // 저장 데이터 Json화
        string jsonData = JsonUtility.ToJson(data, true);
        if(jsonData.Length == 0)
        {
            Debug.LogWarning("[SaveManager] Json 데이터가 비어 있습니다.");
            return;
        }

        // 해당 경로에 Json 문자열 덮어씌우기
        File.WriteAllText(path, jsonData);
        Debug.Log($"[SaveManager] {slotIndex}번 슬롯에 게임 저장 완료");
    }

    /// <summary>
    /// 글로벌 인덱스를 가져와서 실제 로드
    /// </summary>
    private void LoadGameFromGlobalIndex(int globalIndex)
    {
        UpdateSavebles();

        // 저장 슬롯에서 게임 데이터 가져오기
        GameData data = GetGameDataFromSlot(globalIndex);

        // 게임 내 데이터 로드
        foreach (ISaveable saveable in saveables)
        {
            saveable.LoadData(data);
        }

        // 플레이어 위치 로드
        MapManager.Instance.Player.SetPlayerInBase(data.mapLinks.currentMapIndex == 0);
        MapManager.Instance.MoveToMap(data.mapLinks.currentMapIndex, false);
        MapManager.Instance.Player.transform.position = data.playerPosition;

        Debug.Log($"[SaveManager] {globalIndex}번 슬롯에서 게임 불러오기 완료");
    }
    
    /// <summary>
    /// 저장 슬롯에서 게임 로드<br/>
    /// ※ 로딩씬을 거쳐서 로드<br/>
    /// ※ slotIndex 공백일 경우에는 새 게임 시작
    /// </summary>
    public void LoadGameFromSlot(int slotIndex = -1)
    {
        GlobalState.Index = slotIndex;
        if(GlobalState.HasPlayedOpening)
            LoadingSceneController.LoadScene("MainScene");
        else
            LoadingSceneController.LoadScene("OpeningScene");
    }

    /// <summary>
    /// 자동 저장 슬롯에서 게임 로드<br/>
    /// ※로딩씬을 거쳐서 로드
    /// </summary>
    public void LoadGameFromAutoSlot()
    {
        GlobalState.Index = 99;
        LoadingSceneController.LoadScene("MainScene");
    }

    /// <summary>
    /// 저장 슬롯에서 게임 데이터 로드
    /// </summary>
    public GameData GetGameDataFromSlot(int slotIndex)
    {
        // 경로 가져오기
        string path = GetFilePathForSlot(slotIndex);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveManager] {slotIndex}번 슬롯에 저장된 게임 파일이 없습니다.");
            return null;
        }

        // 해당 경로에 Json 파일 가져와서 게임 데이터로 변환
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<GameData>(jsonData);
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
