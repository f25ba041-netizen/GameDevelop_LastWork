using UnityEngine;
using UnityEngine.UI;

public class CircleMove : MonoBehaviour
{
    public float time = 2f;
    private RectTransform rectTransform;
    private Vector3 scale = Vector3.one;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(0,250,0);
        scale = rectTransform.localScale;
    }
    // Update is called once per frame
    void Update()
    {
        rectTransform.localScale = rectTransform.localScale - (scale * Time.deltaTime / time);
        if(rectTransform.localScale.x <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
