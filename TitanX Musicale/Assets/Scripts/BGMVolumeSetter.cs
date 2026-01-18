using UnityEngine;

public class BGMVolumeSetter : MonoBehaviour
{
    private AudioSource source;
    private float beforeVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = GameManager.Instance.settingData.bgmVolume;
        beforeVolume = GameManager.Instance.settingData.bgmVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if (beforeVolume == GameManager.Instance.settingData.bgmVolume) return;
        source.volume = GameManager.Instance.settingData.bgmVolume;
        beforeVolume = GameManager.Instance.settingData.bgmVolume;
    }
}
