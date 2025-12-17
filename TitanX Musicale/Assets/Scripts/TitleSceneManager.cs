using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    private bool isOpening = true;
    public GameObject openingTextList;
    public Animator TitanAnimator;
    public GameObject openingPanel;
    public GameObject titlePanel;
    public GameObject confirmWindow;
    public GameObject continueButton;

    void Start()
    {
        if (GameManager.Instance.saveData == null) continueButton.SetActive(false);
        openingPanel.SetActive(true);
        titlePanel.SetActive(false);
        confirmWindow.SetActive(false);
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
}
