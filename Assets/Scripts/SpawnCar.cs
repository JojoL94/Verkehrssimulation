using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//Script to spawn cars at given spawner Waypoint in defined intervals
public class SpawnCar : MonoBehaviour
{

    //Variable to define in seconds the time between spawning cars
    public float spawnCountdown = 2;

    //Variable to define in seconds the current time of the countdown
    public float currentTime;

    void Start()
    {
        //
        currentTime = spawnCountdown;
    }

    // Update is called once per frame
    void Update()
    {
        //Begin of timer
        if (currentTime > 0)
        {
            //Timer runs down...
            currentTime -= Time.deltaTime;
        }
        else 
        {
            //...start of action after timer stopped
            currentTime = spawnCountdown;
        }
    }
}
