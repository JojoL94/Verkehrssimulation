using System.Collections;
using System.Collections.Generic;
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

    public GameObject lastLocalWaypoint, nextLocalWaypoint;
    
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
        brakeDeceleration = baseAcceleration * 2;
    }


    // Update is called once per frame
    void Update()
    {
        //Move to neighbouring Waypoint
        transform.position = Vector3.MoveTowards(transform.position, nextLocalWaypoint.transform.position,
            speed * Time.deltaTime);
        //If neighbouring Waypoint was reached...
        if (Vector3.Distance(transform.position, nextLocalWaypoint.transform.position) < 0.3)
        {
            lastLocalWaypoint = nextLocalWaypoint;
            getNextLocalWaypoint();
            
            //...check if destination was reached and...
            if (travelRoute.Count == 0)
            {
                //...Despawn car...
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
                if (waypoint != null)
                    if (nexBigWaypoint != null)
                        for (int i = 0; i < nexBigWaypoint.transform.childCount; i++)
                        {
                            if (waypoint.transform == nexBigWaypoint.transform.GetChild(i).transform)
                            {
                                nextLocalWaypoint = waypoint;
                            }
                        }
            }

        }



        return null;
    }


    
    // Is called by intersection and its BoxColliders
    public void giveWait(float distanceToHaltelinie, float left, float right)
    {
        // Breaking is harder, the closer to the Haltelinie, the harder breake.
        speed -= (speed + (brakeDeceleration * Time.deltaTime))  * (left / (distanceToHaltelinie + right));
        // Stellen Sie sicher, dass die Geschwindigkeit nicht unter 0 fällt.
        speed = Mathf.Max(speed, 0);

    }
}