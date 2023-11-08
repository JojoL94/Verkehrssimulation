using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;


//Script for movement of cars
public class MoveCar : MonoBehaviour
{
    //Spawn point of car
    public Transform origin;

    //End point of car
    public Transform destination;

    //List containing all Waypoints the car takes to reach destination
    public List<Transform> travelRoute = new List<Transform>();

    //Maximum speed of car
    public float maxSpeed = 3f;

    //Base acceleration speed of car in Unity units per second
    //It determines, along with Time.deltaTime, the increase of acceeration
    public float baseAcceleration = 5f;

    //Current speed of car
    public float speed = 0f;

    public GameObject lastLocalWaypoint, nextLocalWaypoint, next2LocalWaypoint;

    //GameManager to collect information effecting the whole level
    public GameObject gameManager;

    //Variables for Braking
    public bool doBrake = false;
    public float brakeDeceleration = 10f;
    private float timer;

    public float brakeTimer = 3f;
    // Need to give wait
    public bool doGiveWait = false;

    //Variables for Distance
    private CarDetection myCarDetector;


    //Fixed Update is used for physics calculations that aren't linear
    private void FixedUpdate()
    {
        //Acceleration
        if (speed < maxSpeed && !doBrake)
        {
            //In brackets: Calculate the acceleration of speed
            //Multiply result by Time.deltaTime to get acceleration per seconds
            speed += (speed + (Time.deltaTime * baseAcceleration)) * Time.deltaTime;

            //Ensure, that speed is never bigger than maxSpeed
            speed = Mathf.Clamp(speed, 0, maxSpeed);
        }
        if (doBrake)
        {
            timer += Time.deltaTime;
            if (timer >= brakeTimer)
            {
                doBrake = false;
                timer = 0f;
            }
            // Verringere die Geschwindigkeit basierend auf der Bremsdeceleration
            speed -= (speed + (brakeDeceleration * Time.deltaTime)) * Time.deltaTime;

            // Stelle sicher, dass die Geschwindigkeit nicht negativ wird (rückwärts fahren)
            speed = Mathf.Max(speed, 0f);
        }
        else
        {
            timer = 0f;
        }
    }

    private void Start()
    {
        lastLocalWaypoint = origin.GetChild(0).gameObject;
        nextLocalWaypoint = lastLocalWaypoint.GetComponent<LocalWaypoint>().connectedWaypoints[0];
        myCarDetector = GetComponent<CarDetection>();
        brakeDeceleration = baseAcceleration * 7;
    }

    public bool turnsRight;
    // Update is called once per frame
    void Update()
    {
        //Move to neighbouring Waypoint
        transform.position = Vector3.MoveTowards(transform.position, nextLocalWaypoint.transform.position,
            speed * Time.deltaTime);
        //If neighbouring Waypoint was reached...
        if (Vector3.Distance(transform.position, nextLocalWaypoint.transform.position) < 0.3)
        {
            int lastWaypointIndex = int.Parse(Regex.Replace(lastLocalWaypoint.name, "[^0-9]", ""));
            
            lastLocalWaypoint = nextLocalWaypoint;
            getNextLocalWaypoint();

            int nextWaypointIndex = int.Parse(Regex.Replace(nextLocalWaypoint.name, "[^0-9]", ""));
            int next2WaypointIndex = int.Parse(Regex.Replace(next2LocalWaypoint.name, "[^0-9]", ""));

            int difference = next2WaypointIndex - lastWaypointIndex;
            if (difference == 1 || difference == 2)
                turnsRight = true;
            else turnsRight = false;
            Debug.Log($"Fährt von {lastWaypointIndex} über {nextWaypointIndex} zu {next2WaypointIndex} difference = {difference}");

            //...check if destination was reached and...
            if (travelRoute.Count == 0)
            {
                //...Despawn car
                gameManager.GetComponent<GameManager>().currentCars--;
                Destroy(this.gameObject);
            }
            myCarDetector.SwitchLane();
        }
        //Rotate Object towards driving direction
        transform.LookAt(nextLocalWaypoint.transform);
        transform.Rotate(0, -90, 0);
    }


    public GameObject nexBigWaypoint;

    GameObject getNextLocalWaypoint()
    {
        LocalWaypoint lastWaypoint = lastLocalWaypoint.GetComponent<LocalWaypoint>();
        nexBigWaypoint = travelRoute[0].gameObject;

        

        //Bool to toggle iteration foreach lopp
        bool toggleLoop = true;

        //Look maximum of three main Waypoints in advance to predict route to take in crossing (to switch lanes if necessary)
        Transform thirdNextWaypoint = null;
        if(travelRoute.Count > 2){
            thirdNextWaypoint = travelRoute[2];
        }

        //Check if main Waypoint was reached
        if (nextLocalWaypoint.transform.parent == nexBigWaypoint.transform)
        {
            //Remove reached Waypoint
            travelRoute.Remove(travelRoute[0]);
        }

        //Check if next localWaypoint is connected to a main Waypoint
        if (lastWaypoint.connectedWaypoints[0].transform.parent.GetComponent<Waypoint>() == null)
        {
            nextLocalWaypoint = lastWaypoint.connectedWaypoints[0];
        }
        else
        {

            foreach (GameObject waypoint in lastWaypoint.connectedWaypoints)
            {
                if (waypoint != null && toggleLoop == true && nexBigWaypoint != null)
                    //if (nexBigWaypoint != null)
                        for (int i = 0; i < nexBigWaypoint.transform.childCount; i++)
                        {
                            if (waypoint.transform == nexBigWaypoint.transform.GetChild(i).transform && toggleLoop == true)
                            {
                                //If possible: Look three steps ahead, to check if lane switch is necessary at crossings
                                if (thirdNextWaypoint != null && toggleLoop == true)
                                {
                                    foreach (GameObject localWaypoint in waypoint.GetComponent<LocalWaypoint>().connectedWaypoints) 
                                    {
                                        foreach (GameObject finalLocalWaypoint in localWaypoint.gameObject.GetComponent<LocalWaypoint>().connectedWaypoints) {
                                            if (finalLocalWaypoint.transform.parent == thirdNextWaypoint && toggleLoop == true)
                                            {
                                                toggleLoop = false;
                                                nextLocalWaypoint = waypoint;
                                                break;
                                            }

                                        }
                                        
                                    }    
                                }
                                else 
                                {
                                    nextLocalWaypoint = waypoint;
                                }                                                                
                            }
                        }
            }
        }

        if (travelRoute.Count > 1)
            if (travelRoute[1] != null)
                next2LocalWaypoint = nextLocalWaypoint.GetComponent<LocalWaypoint>().connectedWaypoints[0];

        return null;
    }


    
    // Is called by intersection and its BoxColliders
    public void giveWait(float distanceToHaltelinie, float left, float right)
    {
        if (!turnsRight)
        {
            // Breaking is harder, the closer to the Haltelinie, the harder breake.
            speed -= (speed + (brakeDeceleration * Time.deltaTime))  * (left / (distanceToHaltelinie + right));
            // Stellen Sie sicher, dass die Geschwindigkeit nicht unter 0 fällt.
            speed = Mathf.Max(speed, 0);
        }
        else
        {
            Debug.Log("Eigentliche warten, aber fährt nach rechts, deswegen egal");
        }

    }
}