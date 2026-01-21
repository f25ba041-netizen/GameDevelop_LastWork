#if UNITY_EDITOR
using UnityEditor.SearchService;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class SettingSceneManager : MonoBehaviour
{
    public Slider bgmVolumeSlider;
    public Slider seVolumeSlider;
    public Slider delaySlider;
    public AudioSource ButtonSE;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        bgmVolumeSlider.value = GameManager.Instance.settingData.bgmVolume;
        seVolumeSlider.value = GameManager.Instance.settingData.seVolume;
        delaySlider.value = GameManager.Instance.settingData.delay;
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    { // 指定時間後に渡した関数が実行される
        yield return new WaitForSeconds(waitTime);
        action();
    }

    public void backButton()
    {
        ButtonSE.Play();
        StartCoroutine(DelayMethod(0.1f,() => { // SE分の遅延
            GameManager.Instance.save();
            SceneManager.LoadScene(GameManager.Instance.beforeSceneName);
        }));
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
