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
    public GameObject resultPanel;
    public Text resultEvalText;
    public string musicID;
    public int score;
    private bool isPose = false;
    public AxionMovement axion;
    public bool isRight = false;

    public void moveToRight(){
        axion.moveToRight();
        isRight = true;
    }

    public void moveToLeft(){
        axion.moveToLeft();
        isRight = false;
    }

    public void attack(){
        axion.attack();
        // まだエフェクトのみ 判定の処理を付ける
    }

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

    public void nextButton(){
        GameManager.Instance.selectStory();
        GameManager.Instance.loadScene("Story");
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    { // 指定時間後に渡した関数が実行される
        yield return new WaitForSeconds(waitTime);
        action();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posePanel.SetActive(false);
        countdownPanel.SetActive(false);
        resultPanel.SetActive(false);

        Beam();
        StartCoroutine(DelayMethod(2.0f , () => {
            BeamEnd();
        }));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            poseGame();
        }
        if(isPose) return;
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
            moveToLeft();
        }
        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
            moveToRight();
        }
        if (Input.GetMouseButtonDown(0)) {
            attack();
        }
    }

    void Beam(){ // チャージ開始から再生するためチャージ分の時間を考える
        beamParticle.Play();
    }

    void BeamEnd(){
        beamParticle.Stop();
    }
}
