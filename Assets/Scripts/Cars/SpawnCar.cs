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
    //GameManager to collect information effecting the whole level
    public GameObject gameManager;

    //Collection (empty GameObject) the cars are later added as children for better overview
    public Transform carCollection;

    //Variable to define in seconds the time between spawning cars
    public float spawnCountdown = 2;

    //Waypoint acting as spawn point
    public Transform spawnPoint;

    //Waypoint acting as destination for spawned cars (currently hard coded)
    public Transform destination;

    //Array containing all car PreFabs
    public GameObject[] cars;

    private float maxCars;
    //Variable to define in seconds the current time of the countdown
    private float currentTime;

    //Variables for detecting car standing in spawnpoint and for blocking the spawner from spawning cars in to eachother
    private bool blockedSpawn;
    private Transform lastCarSpawned;

    [SerializeField]
    private float minDistanceToNextCar = 4f;

    // Coroutine Update
    Coroutine spawningCars;


    //Function to randomly spawn cars
    void spawnCar()
    {
        //Create a new GameObject consisting of a randomly chosen car in carCollection
        GameObject car = Instantiate(cars[Random.Range(0, cars.Length - 1)], carCollection);

        //Store Skript as variable for performance reasons
        MoveCar moveCar = car.GetComponent<MoveCar>();
        GameManager managerSkript = gameManager.GetComponent<GameManager>();

        car.name = $"Car${managerSkript.carUID}";
        managerSkript.carUID++;
        //save transform of last car that spawned
        lastCarSpawned = car.transform;

        //Fill MoveCar Script with first Waypoint
        moveCar.origin = spawnPoint;

        //Fill MoveCar Script with destination Waypoint
        /**
         -----------------------------------------------
         !!!!Currently hard coded, need change later!!!!
         -----------------------------------------------
         */
        moveCar.destination = destination;

        moveCar.gameManager = gameManager;

        moveCar.initalYValue = car.gameObject.transform.position.y;

        managerSkript.currentCars++;

        //Place cars to spawn point
        car.gameObject.transform.position = car.gameObject.transform.position+spawnPoint.transform.position;
    }
    
    void Start()
    {
        //Initialize currentTime as spawnCountdown
        currentTime = spawnCountdown;
        maxCars = gameManager.GetComponent<GameManager>().maxCars;
        startSpawnCar();
    }

    public void startSpawnCar()
    {
        if (spawningCars == null)
            spawningCars = StartCoroutine(spawnCarAfterTime());
        else
        {
            StopCoroutine(spawningCars);
            spawningCars = StartCoroutine(spawnCarAfterTime());
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Begin of timer
        /*if (currentTime > 0)
        {
            //Timer runs down...
            currentTime -= Time.deltaTime;
        }
        //...start of action after timer stopped
        else
        {
            //spawn and reset timer if it is the first car or the last car spawned have a min distance to the spawner
            if (lastCarSpawned == null || Vector3.Distance(lastCarSpawned.position, spawnPoint.position) > minDistanceToNextCar)
            {
                if (carCollection.childCount < maxCars)
                {
                    //Spawn random car
                    spawnCar();
                }

                //Reset Timer
                currentTime = spawnCountdown;
            }
        }*/
    }

    
    IEnumerator spawnCarAfterTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(spawnCountdown, spawnCountdown * 4));
            if (lastCarSpawned == null || Vector3.Distance(lastCarSpawned.position, spawnPoint.position) > minDistanceToNextCar)
            {
                if (carCollection.childCount < maxCars)
                {
                    //Spawn random car
                    spawnCar();
                }               
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnPoint != null && destination != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(spawnPoint.position, destination.position);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPoint.position, 1f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(destination.position, 1f);
        }
    }
}