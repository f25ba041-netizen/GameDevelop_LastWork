using UnityEngine;

public class MissileMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public float speed = 300; // →約0.4秒で届く
    private float timer = 0;
    private bool beforeAttack = true;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        transform.position -= speed * Time.deltaTime * transform.forward;
        timer += Time.deltaTime;
        if (transform.position.z <= 0 && beforeAttack) {
            beforeAttack = false;
            GameObject obj = GameObject.Find("GameSceneManager");
            GameSceneManager manager = obj.GetComponent<GameSceneManager>();
            manager.score -= (manager.isInverse != manager.isRight ? 100 : 0);
        }
        if(timer > 5) {
            Destroy(gameObject);
        }
    }
}
