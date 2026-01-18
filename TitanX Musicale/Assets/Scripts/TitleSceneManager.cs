using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    private bool isOpening = true;
    public GameObject openingTextList;
    public Animator TitanAnimator;
    public GameObject openingPanel;
    public GameObject titlePanel;
    public GameObject confirmWindow;
    public GameObject continueButtonObject;
    public Image fadePanel;
    public AudioSource ButtonSE;

    void Start()
    {
        if (GameManager.Instance.saveData == null) continueButtonObject.SetActive(false);
        openingPanel.SetActive(true);
        titlePanel.SetActive(false);
        confirmWindow.SetActive(false);
        fadePanel.enabled = false;
    }

    void Update()
    {
        if (isOpening) {
            isOpening = openingUpdate();
            if (Input.GetMouseButtonDown(0)) {
                endOpening();
                isOpening = false;
            }
            return;
        }
    }

    public float openingSpeed = 100;
    private bool openingUpdate(){
        Vector3 texPos = openingTextList.transform.position;
        texPos.y += openingSpeed * Time.deltaTime;
        openingTextList.transform.position = texPos;
        Vector3 texScale = openingTextList.transform.localScale;
        openingTextList.transform.localScale = texScale - Vector3.one * (openingSpeed * Time.deltaTime / 1000f);
        if (texScale.x <= 0.3) {
            endOpening();
            return false;
        }
        return true;
    }
    private void endOpening(){
        TitanAnimator.SetBool("opening", false);
        openingPanel.SetActive(false);
        titlePanel.SetActive(true);
    }

    public void startButton(){
        ButtonSE.Play();
        if (GameManager.Instance.saveData!=null) {
            confirmWindow.SetActive(true);
            return;
        }
        GameManager.Instance.createNewData();
        StartCoroutine(FadeOutAndLoadScene("Story"));
    }

    public void confirmYesButton(){
        ButtonSE.Play();
        GameManager.Instance.createNewData();
        StartCoroutine(FadeOutAndLoadScene("Story"));
    }

    public void confirmNoButton(){
        ButtonSE.Play();
        confirmWindow.SetActive(false);
    }

    public float fadeDuration = 1f;

    public IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        fadePanel.enabled = true;                 // パネルを有効化
        float elapsedTime = 0.0f;                 // 経過時間を初期化
        Color startColor = fadePanel.color;       // フェードパネルの開始色を取得
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f); // フェードパネルの最終色を設定

        // フェードアウトアニメーションを実行
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;                        // 経過時間を増やす
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);  // フェードの進行度を計算
            fadePanel.color = Color.Lerp(startColor, endColor, t); // パネルの色を変更してフェードアウト
            yield return null;                                     // 1フレーム待機
        }

        fadePanel.color = endColor;  // フェードが完了したら最終色に設定
        GameManager.Instance.loadScene(sceneName); // シーンをロードしてメニューシーンに遷移
    }

    public void continueButton(){
        ButtonSE.Play();
        if (GameManager.Instance.saveData == null) {GameManager.Instance.createNewData();}
        if (GameManager.Instance.saveData.gameStatus == GameStatus.Story){
            StartCoroutine(FadeOutAndLoadScene("Story"));
            return;
        }
        else {
            StartCoroutine(FadeOutAndLoadScene("Game"));
            return;
        }
    }

    public void settingButton(){
        ButtonSE.Play();
        StartCoroutine(FadeOutAndLoadScene("Setting"));
    }
}
