using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Linq;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor.TerrainTools;
#endif

public enum Score
{
    S,
    A,
    B,
    C,
}

public class GameSceneManager : MonoBehaviour
{
    public ParticleSystem beamParticle;
    public GameObject pausePanel;
    public GameObject countdownPanel;
    public Text countdownText;
    public GameObject resultPanel;
    public Text resultEvalText;
    private Score _result = Score.C;
    public Score result{
        get{
            return _result;
        }
        set{
            _result = value;
            resultEvalText.text = $"評価 : {_result}";
        }
    }
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
    public bool isPause = true;
    public AxionMovement axion;
    public bool isRight = false;
    public bool isInverse = false;
    public GameObject missilePrefab;
    public GameObject missilePos;
    public GameObject TitanX;
    public GameObject circlePrefab;
    public Text scoreText;
    private float beamTimer = 0;
    private bool existBeam = false;
    private float bpm;
    private float currentTime = 0f;
    private float currentBeat = 0f;
    private Beat beat;
    private List<CircleMove> currentCircleList = new List<CircleMove>();
    public AudioSource ButtonSE;
    public AudioSource BGM;
    public AudioSource BeamSE;
    public AudioSource DamageSE;
    public AudioSource AttackSE;
    public AudioSource AttackMissSE;

    private enum NoteType
    {
        None,
        Attak,
        Beam,
        Missile,
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
        currentCircleList.RemoveAll(item => item == null);
        if (currentCircleList.Count <= 0) {
            AttackMissSE.Play();
            return;
        }
        float circleTimer = currentCircleList[0].timer;
        if (circleTimer < 0.7) {
            AttackMissSE.Play();
            return;
        }

        // +- 0.3 ミス
        Destroy(currentCircleList[0].transform.gameObject);
        if (circleTimer > 1.2 || circleTimer < 0.8) {
            AttackMissSE.Play();
            return;
        }

        // +- 0.2 成功
        axion.attack(); // エフェクト
        AttackSE.Play();
        score += 100;
        if (circleTimer > 1.1 || circleTimer < 0.9) return;

        // +- 0.1 パーフェクト
        score += 50;
    }

    public void pauseGame(){ // ポーズしてポーズメニューを表示
        ButtonSE.Play();
        if (beat.IsEnd()) return;
        if (isPause) return;
        isPause = true;
        pausePanel.SetActive(true);
    }

    public void settingButton(){
        ButtonSE.Play();
        StartCoroutine(DelayMethod(0.1f,()=>{ // SE分の遅延
            GameManager.Instance.loadScene("Setting");
        }));
        
    }

    public void restartButton(){
        ButtonSE.Play();
        StartCoroutine(DelayMethod(0.1f,()=>{ // SE分の遅延
            GameManager.Instance.loadScene("Game");
        }));
        
    }

    public void titleBackButton(){
        ButtonSE.Play();
        StartCoroutine(DelayMethod(0.1f,()=>{ // SE分の遅延
            GameManager.Instance.loadScene("Title");
        }));
    }

    public void continueButton(){
        ButtonSE.Play();
        countdownPanel.SetActive(true);
        pausePanel.SetActive(false);
        // 3秒カウントダウン表示して戻る
        countdownText.text = "3";
        StartCoroutine(DelayMethod(3.0f , () => {
            isPause = false;
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
        ButtonSE.Play();
        GameManager.Instance.selectStory(result);
        StartCoroutine(DelayMethod(0.1f,()=>{ // SE分の遅延
            GameManager.Instance.loadScene("Story");
        }));
    }

    private void evalScore(){ // C～Sの評価を決定
        float rate = (float)score/beat.maxScore;
        if (rate > 0.8){
            result = Score.S;
            return;
        }
        if (rate > 0.6){
            result = Score.A;
            return;
        }
        if (rate > 0.4){
            result = Score.B;
            return;
        }
        result = Score.C;
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    { // 指定時間後に渡した関数が実行される
        yield return new WaitForSeconds(waitTime);
        action();
    }

    void Start()
    {
        pausePanel.SetActive(false);
        countdownPanel.SetActive(false);
        resultPanel.SetActive(false);

        // 曲のIDを見てモデルをボロボロにするか決める
        if ( GameManager.Instance.saveData.statusID == StatusID.music2_lose ) axion.toBroken();
        if ( GameManager.Instance.saveData.statusID == StatusID.music3_lose ) axion.toBroken();

        NotesData notes = GameManager.Instance.notesData;
        BGM.clip = Resources.Load<AudioClip>($"MusicData/{notes.metadata.title}"); // 音楽ファイル読み込み
        bpm = notes.bpms[0][1];
        beat = new Beat();
        StartCoroutine(DelayMethod(3f , () => { // 3秒後に開始
            isPause = false;
        }));
        StartCoroutine(DelayMethod(3f + GameManager.Instance.settingData.delay * 2 , () => { // 遅延を反映させてBGMを再生
            BGM.Play();
        }));
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            pauseGame();
        }
        if(isPause) return;

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
            if (isInverse == isRight) DamageSE.Play();
        }
        beamTimer += Time.deltaTime;

        if (beat.IsEnd())
        {
            StartCoroutine(DelayMethod(2f, () => { // 2秒後にリザルトへ
                evalScore();
                resultPanel.SetActive(true);
                isPause = true;
            }));
            return;
        }

        if (currentBeat >= beat.current.tick)
        {
            switch (beat.current.beatType)
            {
                case BeatType.Tap:
                    ShowAttackRing();
                    break;
                case BeatType.Critical:
                    TitanXSpin();
                    break;
                case BeatType.Directional:
                    GenerateMissile();
                    break;
                case BeatType.SlideStart:
                    ShowBeam();
                    break;
                case BeatType.SlideEnd:
                    BeamEnd();
                    break;
            }
            beat.Next();
        }
    }

    class Beat
    {
        public List<Note> flow;
        public Note current = null;
        public Note next = null;
        private int currentIndex = 0;
        public int maxScore;
        public Beat()
        {
            flow = new List<Note>();
            Init();
        }

        private void Init()
        {
            int safety = 0;
            NotesData notes = JsonConvert.DeserializeObject<NotesData>(JsonConvert.SerializeObject(GameManager.Instance.notesData));
            // タップからフリックに対応するノーツを削除
            foreach(var note in notes.directionals){
                for(int i=0;i<notes.taps.Count;i++){
                    if(notes.taps[i].tick != note.tick)continue;
                    notes.taps.RemoveAt(i);
                    break;
                }
            }

            maxScore = notes.taps.Count * 150;

            while (notes.taps.Count > 0 || notes.directionals.Count > 0 || notes.slides.Count > 0)
            { // ノーツを読み込んで時間順にflowに入れる
                // 無限ループを回避
                safety++;
                if (safety > 10000)
                {
                    Debug.LogError("Infinite loop detected");
                    break;
                }

                // 各種類のノーツで一番早いものを仮リストに入れる
                List<Note> tempList = new List<Note>();
                Note slideEnd = null;
                if(notes.taps.Count > 0)
                {
                    if(notes.taps[0].type == 1)
                    {
                        notes.taps[0].beatType = BeatType.Tap;
                    }
                    else
                    {
                        notes.taps[0].beatType = BeatType.Critical;
                    }
                    tempList.Add(notes.taps[0]);
                }
                if(notes.directionals.Count > 0)
                {
                    notes.directionals[0].beatType = BeatType.Directional;
                    tempList.Add(notes.directionals[0]);
                }
                if(notes.slides.Count > 0)
                {
                    notes.slides[0][0].beatType = BeatType.SlideStart;
                    notes.slides[0][1].beatType = BeatType.SlideEnd;
                    tempList.Add(notes.slides[0][0]);
                    slideEnd = notes.slides[0][1];
                }
                if (tempList.Count == 0)
                {
                    Debug.LogError("tempList is empty. Data broken?");
                    break;
                }

                // 仮リストから一番早いものを選んでflowに追加、元のリストからは削除
                int minIndex = tempList
                    .Select((note, index) => new { note.tick, index })
                    .OrderBy(x => x.tick)
                    .First()
                    .index;
                switch (tempList[minIndex].beatType)
                {
                    case BeatType.Tap:
                        tempList[minIndex].tick -= (int) notes.bpms[0][1] * 8; // 開始タイミングの調整
                        flow.Add(tempList[minIndex]);
                        notes.taps.RemoveAt(0);
                        break;
                    case BeatType.Critical:
                        flow.Add(tempList[minIndex]);
                        notes.taps.RemoveAt(0);
                        break;
                    case BeatType.Directional:
                        tempList[minIndex].tick -= (int) (notes.bpms[0][1] * 8 * 0.4); // 開始タイミングの調整
                        flow.Add(tempList[minIndex]);
                        notes.directionals.RemoveAt(0);
                        break;
                    case BeatType.SlideStart:
                        tempList[minIndex].tick -= (int) notes.bpms[0][1] * 8; // 開始タイミングの調整
                        flow.Add(tempList[minIndex]);
                        flow.Add(slideEnd);
                        notes.slides.RemoveAt(0);
                        break;
                    default:
                        Debug.LogError("Unknown BeatType: " + tempList[minIndex].beatType);
                        break;
                }
            }

            // 結局タイミングの調整とかでズレるから並べ直す
            flow = flow.OrderBy(p => p.tick).ToList();

            if (flow.Count < 1) return;
            current = flow[0];

            if (flow.Count < 2) return;
            next = flow[1];
        }
        
        public void Next()
        {
            if (next != null)
            {
                current = next;
            }
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
            if (currentIndex >= flow.Count) return true;
            return false;
        }
    }

    void ShowAttackRing()
    {
        GameObject circle = Instantiate(circlePrefab);
        currentCircleList.Add(circle.GetComponent<CircleMove>());
    }

    void ShowBeam(){
        // チャージ開始から再生するためチャージ分の時間を考える
        // チャージには1秒かかる
        beamParticle.Play();
        StartCoroutine(DelayMethod(1 , () => {
            BeamSE.Play();
            existBeam = true;
        }));
    }

    void BeamEnd(){
        beamParticle.Stop();
        existBeam = false;
    }

    void GenerateMissile(){ // 0.4秒前に
        GameObject missileObj = Instantiate(missilePrefab);
        missileObj.transform.position = missilePos.transform.position;
        missileObj.transform.rotation = missilePos.transform.rotation;
    }

    void TitanXSpin(){
        isInverse = !isInverse;
        // 0.09秒かけて回転
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
