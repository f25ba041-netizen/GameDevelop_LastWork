using UnityEngine;

public class MissileMove : MonoBehaviour
{
    private GameSceneManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    public float speed = 300; // →約0.4秒で届く
    private float timer = 0;
    private bool beforeAttack = true;
    // Update is called once per frame
    void Update()
    {
        if (manager.isPause) {
            return;
        }
        Vector3 pos = transform.position;
        transform.position -= speed * Time.deltaTime * transform.forward;
        timer += Time.deltaTime;
        if (transform.position.z <= 0 && beforeAttack) {
            beforeAttack = false;
            GameObject obj = GameObject.Find("GameSceneManager");
            GameSceneManager manager = obj.GetComponent<GameSceneManager>();
            manager.score -= (manager.isInverse != manager.isRight ? 100 : 0);
            if (manager.isInverse != manager.isRight) GameObject.Find("DamageSE").GetComponent<AudioSource>().Play();
        }
        if(timer > 5) {
            Destroy(gameObject);
        }
    }
}
