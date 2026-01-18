using UnityEngine;

public class AttackParticleRemove : MonoBehaviour
{
    private float count = 0;
    private GameSceneManager manager;
    private ParticleSystem particle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        particle = GetComponent<ParticleSystem>();
    }
    private bool beforePause = false;
    // Update is called once per frame
    void Update()
    {
        if (beforePause && !manager.isPause) particle.Play();
        beforePause = manager.isPause;
        if (manager.isPause) {
            particle.Pause();
            return;
        }
        count += Time.deltaTime;
        if (count > 3) {
            Destroy(this.gameObject);
        }
    }
}
