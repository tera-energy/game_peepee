using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimater : MonoBehaviour
{

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    void Start()
    {
        lastTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {

        float deltaTime = Time.realtimeSinceStartup - (float)lastTime;

        particle.Simulate(deltaTime, true, false); //last must be false!!

        lastTime = Time.realtimeSinceStartup;
    }

    private double lastTime;
    private ParticleSystem particle;

}

    
