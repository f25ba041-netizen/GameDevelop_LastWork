using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingSceneManager : MonoBehaviour
{
    public Slider bgmVolumeSlider;
    public Slider seVolumeSlider;
    public Slider delaySlider;

    private void Awake()
    {
        bgmVolumeSlider = GetComponent<Slider>();
        seVolumeSlider = GetComponent<Slider>();
        delaySlider = GetComponent<Slider>();
        Init();
    }

    private void Init()
    {
        bgmVolumeSlider.value = GameManager.Instance.settingData.bgmVolume;
        seVolumeSlider.value = GameManager.Instance.settingData.seVolume;
        delaySlider.value = GameManager.Instance.settingData.delay;
    }
    public void backButton()
    {
        GameManager.Instance.save();
        SceneManager.LoadScene(GameManager.Instance.beforeSceneName);
    }

    public void OnChangedBGMVolume()
    {
        GameManager.Instance.settingData.bgmVolume = bgmVolumeSlider.value;
    }

    public void OnChangedSEVolume()
    {
        GameManager.Instance.settingData.seVolume = seVolumeSlider.value;
    }

    public void OnChangedDelayVolume()
    {
        GameManager.Instance.settingData.delay = delaySlider.value;
    }
}
