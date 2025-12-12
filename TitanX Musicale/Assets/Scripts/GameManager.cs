using UnityEngine;

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
    private SaveData _data;
    public SaveData data{
        get {
            if (_data == null) _data = loadData();
            return _data;
        }
    }
    private SaveData loadData(){
        return null;
    }
}

public class SaveData{

}