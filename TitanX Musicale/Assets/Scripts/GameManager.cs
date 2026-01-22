#if UNITY_EDITOR
using UnityEditor.SearchService;
#endif
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
            // 続きからで起動したとき、ノーツのデータがまだ読み込まれていない可能性がある
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
        if (saveData.statusID == StatusID.story_endA || saveData.statusID == StatusID.story_endB) return false;
        if (saveData.statusID == StatusID.story1) saveData.statusID = StatusID.music1_common;
        if (saveData.statusID == StatusID.story2A) saveData.statusID = StatusID.music2_win;
        if (saveData.statusID == StatusID.story2B) saveData.statusID = StatusID.music2_lose;
        if (saveData.statusID == StatusID.story3A) saveData.statusID = StatusID.music3_win;
        if (saveData.statusID == StatusID.story3B) saveData.statusID = StatusID.music3_lose;

        _notesData = loadNotesData($"{saveData.statusID}");
        saveData.gameStatus = GameStatus.Game;
        return true;
    }

    public void selectStory(Score score){
        bool win = (score == Score.A || score == Score.S);
        if (saveData.statusID == StatusID.music1_common && win) saveData.statusID = StatusID.story2A;
        if (saveData.statusID == StatusID.music1_common && !win) saveData.statusID = StatusID.story2B;
        if (saveData.statusID == StatusID.music2_win && win) saveData.statusID = StatusID.story3A;
        if (saveData.statusID == StatusID.music2_lose || (saveData.statusID == StatusID.music2_win && !win)) saveData.statusID = StatusID.story3B;
        if (saveData.statusID == StatusID.music3_win && win) saveData.statusID = StatusID.story_endA;
        if (saveData.statusID == StatusID.music3_lose || (saveData.statusID == StatusID.music3_win && !win)) saveData.statusID = StatusID.story_endB;

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
    music1_common,
    music2_win,
    music2_lose,
    music3_win,
    music3_lose,
    story1,
    story2A,
    story2B,
    story3A,
    story3B,
    story_endA,
    story_endB,
}

[System.Serializable]
public class SaveData{
    public Difficulty difficulty = Difficulty.Normal;
    public GameStatus gameStatus = GameStatus.Story;
    public StatusID statusID = StatusID.story1;

    public string toJson()
    {
        string jsonText = JsonUtility.ToJson(this);
        return jsonText;
    }
}

[System.Serializable]
public class SettingData
{
    private float _bgmVolume = 1;
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

    private float _seVolume = 1;
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

    private float _delay = 0;
    public float delay
    {
        get
        {
            return _delay;
        }
        set
        {
            if (value < -1 || value > 1) return;
            _delay = value;
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