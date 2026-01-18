using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;
using Newtonsoft.Json;
public class GameManager
{
    static private GameManager instance;
    static public GameManager Instance{
        get {
            if (instance == null) instance = new GameManager();
            return instance;
        }
    }
    private GameManager(){}
    private SaveData _saveData;
    public SaveData saveData{
        get {
            if (_saveData == null) _saveData = loadSaveData();
            return _saveData;
        }
        set
        {
            _saveData = value;
        }
    }

    private SettingData _settingData;
    public SettingData settingData
    {
        get
        {
            if (_settingData == null) _settingData = loadSettingData();
            return _settingData;
        }
    }

    private NotesData _notesData;
    public NotesData notesData{
        get{
            if (_notesData == null) _notesData = loadNotesData($"{saveData.statusID}");
            return _notesData;
        }
    }

    public string beforeSceneName = "";

    public void save()
    {
        string saveDataJson = (_saveData != null ? _saveData.toJson() : "");
        string settingDataJson = (_settingData != null ? _settingData.toJson() : "");

        PlayerPrefs.SetString("saveDataJson", saveDataJson);
        PlayerPrefs.SetString("settingDataJson", settingDataJson);
        PlayerPrefs.Save();
    }
    private SaveData loadSaveData(){
        string json = PlayerPrefs.GetString("saveDataJson");
        SaveData data = json == "" ? null : JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    private SettingData loadSettingData()
    {
        string json = PlayerPrefs.GetString("settingDataJson");
        SettingData data = json == "" ? new SettingData() : JsonUtility.FromJson<SettingData>(json);
        return data;
    }

    public void createNewData()
    {
        saveData = new SaveData();
    }

    public void loadScene(string nextScene)
    {
        beforeSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(nextScene);
    }

    private NotesData loadNotesData(String MusicID){
        TextAsset asset = Resources.Load<TextAsset>($"MusicData/{MusicID}");
        string jsonText = asset?.text;
        if(jsonText == null)
        {
            Debug.LogError("TextAsset が読み込めませんでした");
            return null;
        }
        return JsonConvert.DeserializeObject<NotesData>(jsonText);
    }

    public bool selectMusic(){ // 選曲が失敗したとき(=エンディングのとき)falseを返す
        // !
        // ここに選曲処理を書く
        // !

        //テスト用
        saveData.statusID = StatusID.music_test;

        _notesData = loadNotesData($"{saveData.statusID}");
        saveData.gameStatus = GameStatus.Game;
        return true;
    }

    public void selectStory(){
        // !
        // ここにストーリー選択の処理を書く
        // !

        saveData.gameStatus = GameStatus.Story;
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Difficult,
}

public enum GameStatus
{
    Story,
    Game,
}

public enum StatusID
{
    test,
    music_test,
}

[System.Serializable]
public class SaveData{
    public Difficulty difficulty = Difficulty.Normal;
    public GameStatus gameStatus = GameStatus.Story;
    public StatusID statusID = StatusID.test;

    public string toJson()
    {
        string jsonText = JsonUtility.ToJson(this);
        return jsonText;
    }
}

[System.Serializable]
public class SettingData
{
    private float _bgmVolume;
    public float bgmVolume
    {
        get
        {
            return _bgmVolume;
        }
        set
        {
            if (value < 0 || value > 1) return;
            _bgmVolume = value;
        }
    }

    private float _seVolume;
    public float seVolume
    {
        get
        {
            return _seVolume;
        }
        set
        {
            if (value < 0 || value > 1) return;
            _seVolume = value;
        }
    }

    private float _delay;
    public float delay
    {
        get
        {
            return _delay;
        }
        set
        {
            if (value < 0) return;
            _delay = value;
        }
    }

    private float _speed;
    public float speed
    {
        get
        {
            return _speed;
        }
        set
        {
            if (value < 0) return;
            _speed = value;
        }
    }

    public string toJson()
    {
        string jsonText = JsonUtility.ToJson(this);
        return jsonText;
    }
}

[System.Serializable]
public class NotesData {
    public MetaData metadata;
    public List<Note> taps;
    public List<Note> directionals;
    public List<List<Note>> slides;
    public List<List<float>> bpms;
    public List<List<float>> barLengths;
}
[System.Serializable]
public class MetaData {
    public string title;
    public string artist;
    public string designer;
    public float waveoffset;
    public List<string> requests;
}
[System.Serializable]
public class Note {
    public int tick;
    public int lane;
    public int width;
    public int type;
    public BeatType beatType = BeatType.None;
}

public enum BeatType
{
    None,
    Tap,
    Critical,
    Directional,
    SlideStart,
    SlideEnd,
}