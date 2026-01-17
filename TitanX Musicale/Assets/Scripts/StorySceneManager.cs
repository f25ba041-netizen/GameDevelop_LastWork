using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class StoryData
{
    public StorySceneData[] scenes;
}
[System.Serializable]
public class StorySceneData
{
    public string image;
    public SerifData[] serif;
}
[System.Serializable]
public class SerifData
{
    public string speaker;
    public string text;
}

public class StorySceneManager : MonoBehaviour
{
    private string StoryID;
    private StoryData storyData;
    public Image background;
    public Text speaker;
    public Text subtitle;

    public StorySceneManager(){
        StoryID = GameManager.Instance.saveData.statusID.ToString();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string jsonData = Resources.Load<TextAsset>($"StoryData/{StoryID}").ToString();
        storyData = JsonUtility.FromJson<StoryData>(jsonData);
        background.sprite = Resources.Load<Sprite>($"{storyData.scenes[0].image}");
        speaker.text = storyData.scenes[0].serif[0].speaker;
        subtitle.text = storyData.scenes[0].serif[0].text;
    }

    private int currentScene = 0;
    private int currentSerif = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            nextScene();
        }
    }

    private void toBattle(){
        GameManager.Instance.selectMusic();
        if (GameManager.Instance.notesData == null){ // 選曲をしてエンディングだったとき
            GameManager.Instance.saveData = null;
            GameManager.Instance.save();
            GameManager.Instance.loadScene("title");
            return;
        }
        GameManager.Instance.save();
        GameManager.Instance.loadScene("game");
    }

    public void skipButton(){
        toBattle();
    }

    public void nextScene(){
        currentSerif++;
        if (currentSerif >= storyData.scenes[currentScene].serif.Length) {
            currentScene++;
            currentSerif = 0;
        }
        if (currentScene >= storyData.scenes.Length) {
            toBattle();
            return;
        }
        background.sprite = Resources.Load<Sprite>($"{storyData.scenes[currentScene].image}");
        speaker.text = storyData.scenes[currentScene].serif[currentSerif].speaker;
        subtitle.text = storyData.scenes[currentScene].serif[currentSerif].text;
    }
}
