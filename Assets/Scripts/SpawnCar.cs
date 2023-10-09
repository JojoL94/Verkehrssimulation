using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


//Script to spawn cars in defined intervals
public class SpawnCar : MonoBehaviour
{

    //Variable to define in seconds the time between spawning cars
    public float spawnCountdown = 2;

    //Variable to define in seconds the current time of the countdown
    private float currentTime;

    //Array containing all car PreFabs
    private GameObject[] cars;

    //Function to randomly spawn cars
    void spawnCar() {

        //Create a new GameObject consisting of a randomly chosen car in Resources/PreFabs/Cars folder
        GameObject car = Instantiate(cars[Random.Range(0, cars.Length - 1)]);

    }
    void Start()
    {
        //Initialize currentTime as spawnCountdown
        currentTime = spawnCountdown;

        //Load all cars of Cars folder
        cars = Resources.LoadAll<GameObject>("PreFabs/Cars");
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

            //Randomly spawn car
            spawnCar();

            //Reset Timer
            currentTime = spawnCountdown;
        }
    }
}
