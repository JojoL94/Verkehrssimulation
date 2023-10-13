using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


//Script to spawn cars in defined intervals
public class SpawnCar : MonoBehaviour
{

    //Collection (empty GameObject) the cars are later added as children for better overview
    public Transform carCollection;

    //Max number of cars driving at the same time
    public float maxCars = 15;

    //Variable to define in seconds the time between spawning cars
    public float spawnCountdown = 2;

    //Waypoint acting as spawn point
    public Waypoint spawnPoint;

    //Variable to define in seconds the current time of the countdown
    private float currentTime;

    //Array containing all car PreFabs
    private GameObject[] cars;

    //Function to randomly spawn cars
    void spawnCar() {

        //Create a new GameObject consisting of a randomly chosen car in Resources/PreFabs/Cars folder
        GameObject car = Instantiate(cars[Random.Range(0, cars.Length - 1)]);

        //Fill MoveCar Script with first Waypoint
        car.GetComponent<MoveCar>().nextWaypoint = spawnPoint.GetComponent<Waypoint>().nextWaypoints[0];

        //Add car to carCollection

        car.transform.SetParent(carCollection, false);

        //Place cars to spawn point
        car.gameObject.transform.position = spawnPoint.transform.position;

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
        //...start of action after timer stopped
        else
        {
            if (carCollection.childCount < maxCars) {

                //Spawn random car
                spawnCar();
            }

            //Reset Timer
            currentTime = spawnCountdown;
        }
    }
}
