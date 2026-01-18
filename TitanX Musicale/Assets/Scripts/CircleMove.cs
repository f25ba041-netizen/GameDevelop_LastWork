using UnityEngine;
using UnityEngine.UI;

public class CircleMove : MonoBehaviour
{
    private GameSceneManager manager;
    RectTransform rectTransform;
    public float timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.SetSiblingIndex(1); // 判定基準の上に表示
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(-5,250,0);
    }
    // Update is called once per frame
    void Update()
    {
        if (manager.isPause) {
            return;
        }
        timer += Time.deltaTime;
        rectTransform.localScale = rectTransform.localScale - (new Vector3(1,1,1) * Time.deltaTime / 2);
        if(timer > 1.2)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerAttack")
        {
            Destroy(collision.gameObject);
        }
    }
}
