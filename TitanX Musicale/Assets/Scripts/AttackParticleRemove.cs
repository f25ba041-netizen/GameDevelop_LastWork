using UnityEngine;

public class AttackParticleRemove : MonoBehaviour
{
    private float count = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        if (count > 3) {
            Destroy(this.gameObject);
        }
    }
}
