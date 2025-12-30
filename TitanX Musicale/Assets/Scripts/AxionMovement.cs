using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


public class AxionMovement : MonoBehaviour
{
    public ParticleSystem moveParticle;
    private float targetX = -23;
    private float moveSpeed = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(targetX,0,0);
        if (Vector3.Distance(currentPos,targetPos) < moveSpeed * Time.deltaTime){
            transform.position = targetPos;
        }
        else {
            transform.position = currentPos + (targetPos - currentPos) * moveSpeed * Time.deltaTime;
        }
    }

    public void moveToRight(){
        moveParticle.Play();
        targetX = 23;
    }

    public void moveToLeft(){
        moveParticle.Play();
        targetX = -23;
    }
}
