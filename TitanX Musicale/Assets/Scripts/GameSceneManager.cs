using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameSceneManager : MonoBehaviour
{
    public ParticleSystem beamParticle;
    public GameObject posePanel;
    public GameObject countdownPanel;
    public Text countdownText;
    public string musicID;
    public int score;
    private bool isPose = false;

    public void poseGame(){
        isPose = true;
        posePanel.SetActive(true);
    }

    public void settingButton(){
        GameManager.Instance.loadScene("Setting");
    }

    public void restartButton(){
        GameManager.Instance.loadScene("Game");
    }

    public void titleBackButton(){
        GameManager.Instance.loadScene("Title");
    }

    public void continueButton(){
        countdownPanel.SetActive(true);
        posePanel.SetActive(false);
        countdownText.text = "3";
        StartCoroutine(DelayMethod(3.0f , () => {
            isPose = false;
            countdownPanel.SetActive(false);
        }));
        StartCoroutine(DelayMethod(2.0f , () => {
            countdownText.text = "1";
        }));
        StartCoroutine(DelayMethod(1.0f , () => {
            countdownText.text = "2";
        }));
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posePanel.SetActive(false);
        countdownPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape)){
            poseGame();
        }
    }

    void Beam(){
        beamParticle.Play();
    }

    void BeamEnd(){
        // ビームを止める処理
    }
}
