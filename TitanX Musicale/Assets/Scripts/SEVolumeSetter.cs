using UnityEngine;

public class SEVolumeSetter : MonoBehaviour
{
    private AudioSource source;
    private float beforeVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume =  GameManager.Instance.settingData.seVolume;
        beforeVolume = GameManager.Instance.settingData.seVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if (beforeVolume == GameManager.Instance.settingData.seVolume) return;
        source.volume =  GameManager.Instance.settingData.seVolume;
        beforeVolume = GameManager.Instance.settingData.seVolume;
    }
}
