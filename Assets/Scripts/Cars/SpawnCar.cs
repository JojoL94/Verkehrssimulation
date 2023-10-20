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

    //Variable to define in seconds the time between spawning cars
    public float spawnCountdown = 2;

    //Toogle for incomming traffic (car always searches for previous Waypoint)
    //Default: false (car always searches for next Waypoint)
    public bool incommingTraffic = false;

    //Waypoint acting as spawn point
    public Transform spawnPoint;

    //Waypoint acting as destination for spawned cars (currently hard coded)
    public Transform destination;

    //Array containing all car PreFabs
    public GameObject[] cars;

    //Variable to define in seconds the current time of the countdown
    private float currentTime;



    //Function to randomly spawn cars
    void spawnCar() {

        //Create a new GameObject consisting of a randomly chosen car in carCollection
        GameObject car = Instantiate(cars[Random.Range(0, cars.Length - 1)],carCollection);


        //Toggle incomming traffic bool of Pathfinding for each car at spawn
        if (!incommingTraffic)
        {
            car.GetComponent<Pathfinding>().incomingTraffic = false;
        }
        else {
            car.GetComponent<Pathfinding>().incomingTraffic = true;
        }

        //Fill MoveCar Script with first Waypoint
        car.GetComponent<MoveCar>().origin = spawnPoint;

        //Fill MoveCar Script with destination Waypoint
        /**
         -----------------------------------------------
         !!!!Currently hard coded, need change later!!!!
         -----------------------------------------------
         */
        car.GetComponent<MoveCar>().destination = destination;



        //Place cars to spawn point
        car.gameObject.transform.position = spawnPoint.transform.position;

    }



    void Start()
    {
        //Initialize currentTime as spawnCountdown
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
        //...start of action after timer stopped
        else
        {
            if (carCollection.childCount < GameObject.Find("GameManager").GetComponent<GameManager>().maxCars) {

                //Spawn random car
                spawnCar();
            }

            //Reset Timer
            currentTime = spawnCountdown;
        }
    }
}
