using UnityEngine;
using UnityEngine.UI;

public class CircleMove : MonoBehaviour
{
    RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(0,250,0);
    }
    // Update is called once per frame
    void Update()
    {
        rectTransform.localScale = rectTransform.localScale - (new Vector3(1,1,1) * Time.deltaTime / 2);
        if(rectTransform.localScale.magnitude <= 0)
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
