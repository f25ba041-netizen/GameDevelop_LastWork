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
        StoryID = GameManager.Instance.data.statusID.ToString();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"StoryData/{StoryID}.json");
        string jsonData = Resources.Load<TextAsset>($"StoryData/{StoryID}").ToString();
        storyData = JsonUtility.FromJson<StoryData>(jsonData);
        Debug.Log(storyData.scenes[0].image);
        background.sprite = Resources.Load<Sprite>($"{storyData.scenes[0].image}");
        speaker.text = storyData.scenes[0].serif[0].speaker;
        subtitle.text = storyData.scenes[0].serif[0].text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void skipButton(){}

    public void nextScene(){}
}
