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
    public StatusID musicID;
    private int _score = 0;
    public int score{
        set {
            _score = value;
            if (_score < 0) _score = 0; 
            scoreText.text = $"スコア {_score}";
        }
        get {
            return _score;
        }
    }
    private bool isPose = true;
    public AxionMovement axion;
    public bool isRight = false;
    public bool isInverse = false;
    public GameObject missilePrefab;
    public GameObject missilePos;
    public GameObject TitanX;
    public Text scoreText;
    public NotesData notes;
    private float beamTimer = 0;
    private bool existBeam = false;
    private float bpm;

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

        // 本番で適用
        // musicID = GameManager.saveData.StatusID;

        // テスト用
        musicID = StatusID.mtest;

        notes = GameManager.Instance.loadNotesData(musicID);
        Debug.Log(notes.bpms[1]);
        bpm = notes.bpms[1];
        StartCoroutine(DelayMethod(3f , () => { // 3秒後に開始
            isPose = false;
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
        if (beamTimer >= 0.1f && existBeam) {
            beamTimer = 0;
            score -= (isInverse == isRight ? 20 : 0);
        }
        beamTimer += Time.deltaTime;
    }

    void Beam(){
        // チャージ開始から再生するためチャージ分の時間を考える
        // チャージには1秒かかる
        beamParticle.Play();
        StartCoroutine(DelayMethod(1 , () => {
            existBeam = true;
        }));
    }

    void BeamEnd(){
        beamParticle.Stop();
        existBeam = false;
    }

    void Missile(){ // 0.4秒前に
        GameObject missileObj = Instantiate(missilePrefab);
        missileObj.transform.position = missilePos.transform.position;
        missileObj.transform.rotation = missilePos.transform.rotation;
    }

    void Spin(){
        isInverse = !isInverse;
        TitanX.transform.Rotate(new Vector3(0, 0, 1), 180f/4);
        StartCoroutine(DelayMethod(0.03f , () => {
            TitanX.transform.Rotate(new Vector3(0, 0, 1), 180f/4);
        }));
        StartCoroutine(DelayMethod(0.06f , () => {
            TitanX.transform.Rotate(new Vector3(0, 0, 1), 180f/4);
        }));
        StartCoroutine(DelayMethod(0.09f , () => {
            TitanX.transform.Rotate(new Vector3(0, 0, 1), 180f/4);
        }));
    }
}
