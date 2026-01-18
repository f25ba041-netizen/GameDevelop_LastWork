using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor.TerrainTools;

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
    private bool isResult = false;
    public AxionMovement axion;
    public bool isRight = false;
    public bool isInverse = false;
    public GameObject missilePrefab;
    public GameObject missilePos;
    public GameObject TitanX;
    public GameObject circlePrefab;
    public Text scoreText;
    public NotesData notes;
    private float beamTimer = 0;
    private bool existBeam = false;
    private float bpm;
    private float currentTime = 0f;
    private float currentBeat = 0f;
    private Beat beat;
    private EvaluationType evaluation = EvaluationType.None;

    public enum EvaluationType
    {
        None,
        C,
        B,
        A,
        S,
    }

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
        if (isPose) return;
        Time.timeScale = 0f;
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
            Time.timeScale = 1f;
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
        GameManager.Instance.selectStory(evaluation);
        GameManager.Instance.loadScene("Story");
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    { // 指定時間後に渡した関数が実行される
        yield return new WaitForSecondsRealtime(waitTime);
        action();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posePanel.SetActive(false);
        countdownPanel.SetActive(false);
        resultPanel.SetActive(false);

        // 本番で適用
        //musicID = GameManager.saveData.StatusID;

        // テスト用
        /*
        musicID = StatusID.mtest;

        notes = GameManager.Instance.loadNotesData(musicID);
        Debug.Log(notes.bpms[1]);
        */
        notes = GameManager.Instance.notesData;
        bpm = notes.bpms[0][1];
        beat = new Beat();
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

        currentTime += Time.deltaTime;
        currentBeat = currentTime * (bpm * 8);

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
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

        if (beat.current != null)
        {
            if (currentBeat >= beat.current.tick)
            {
                switch (beat.current.beatType)
                {
                    case BeatType.Tap:
                        Attack();
                        break;
                    case BeatType.Critical:
                        Spin();
                        break;
                    case BeatType.Directional:
                        Missile();
                        break;
                    case BeatType.SlideStart:
                        Beam();
                        break;
                    case BeatType.SlideEnd:
                        BeamEnd();
                        break;
                }
                if (beat.IsEnd())
                {
                    StartCoroutine(DelayMethod(1f, () => { // 1秒後にリザルト画面
                        GameManager.Instance.save();
                        resultPanel.SetActive(true);
                    }));
                }
                beat.Next();
            }
        }
        
    }

    class Beat
    {
        public List<Note> flow;
        public Note current = null;
        public Note next = null;
        private int currentIndex = 0;
        public Beat()
        {
            flow = new List<Note>();
            Init();
        }

        private void Init()
        {
            int safety = 0;
            NotesData _notes = JsonConvert.DeserializeObject<NotesData>(JsonConvert.SerializeObject(GameManager.Instance.notesData));
            while (_notes.taps.Count > 0 || _notes.directionals.Count > 0 || _notes.slides.Count > 0)
            {
                safety++;
                if (safety > 10000)
                {
                    Debug.LogError("Infinite loop detected");
                    break;
                }

                List<Note> tempList = new List<Note>();
                Note slideEnd = null;
                if(_notes.taps.Count > 0)
                {
                    if(_notes.taps[0].type == 1)
                    {
                        _notes.taps[0].beatType = BeatType.Tap;
                    }
                    else
                    {
                        _notes.taps[0].beatType = BeatType.Critical;
                    }
                    tempList.Add(_notes.taps[0]);
                }
                if(_notes.directionals.Count > 0)
                {
                    _notes.directionals[0].beatType = BeatType.Directional;
                    tempList.Add(_notes.directionals[0]);
                }
                if(_notes.slides.Count > 0)
                {
                    _notes.slides[0][0].beatType = BeatType.SlideStart;
                    _notes.slides[0][1].beatType = BeatType.SlideEnd;
                    tempList.Add(_notes.slides[0][0]);
                    slideEnd = _notes.slides[0][1];
                }

                if (tempList.Count == 0)
                {
                    Debug.LogError("tempList is empty. Data broken?");
                    break;
                }

                int minIndex = tempList
                    .Select((note, index) => new { note.tick, index })
                    .OrderBy(x => x.tick)
                    .First()
                    .index;

                switch (tempList[minIndex].beatType)
                {
                    case BeatType.Tap:
                        flow.Add(tempList[minIndex]);
                        _notes.taps.RemoveAt(0);
                        break;
                    case BeatType.Critical:
                        flow.Add(tempList[minIndex]);
                        _notes.taps.RemoveAt(0);
                        break;
                    case BeatType.Directional:
                        
                        flow.Add(tempList[minIndex]);
                        _notes.directionals.RemoveAt(0);
                        break;
                    case BeatType.SlideStart:
                        tempList[minIndex].tick -= (int)_notes.bpms[0][1] * 8;
                        flow.Add(tempList[minIndex]);
                        flow.Add(slideEnd);
                        _notes.slides.RemoveAt(0);
                        break;
                    default:
                        Debug.LogError("Unknown BeatType: " + tempList[minIndex].beatType);
                        break;
                }
            }

            if (flow.Count < 1) return;
            current = flow[0];

            if (flow.Count < 2) return;
            next = flow[1];
        }
        
        public void Next()
        {
            current = next;

            currentIndex++;
            if(currentIndex + 1 < flow.Count)
            {
                next = flow[currentIndex + 1];
            }
            else
            {
                next = null;
            }
        }
        public bool IsEnd()
        {
            if (currentIndex >= flow.Count - 1) return true;
            return false;
        }
    }

    void Attack()
    {
        GameObject circle = Instantiate(circlePrefab, TitanX.transform.position, Quaternion.identity);
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
