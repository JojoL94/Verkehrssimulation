using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;


//Script for movement of cars
public class MoveCar : MonoBehaviour
{
    //Identification Variable
    public int carID;
    
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

    //nexBigWaypoint saves the next mainWaypoint
    public GameObject nexBigWaypoint;

    //Variables for Braking
    public bool doBrake = false;
    public float brakeDeceleration = 10f;
    private float timer;

    public float brakeTimer = 3f;

    // Need to give wait
    public bool doGiveWait = false;

    //Variables for Distance
    private CarDetection myCarDetector;

    //Variables for lane switching
    //Timer to check if on target lane
    private float laneTimer;

    //Bool to toggle for- and foreach-loops in lane switching
    private bool laneLooping = true;

    //lokalTargetWaypoint saves the child of the next mainWaypoint => needs to be reached to turn left/right if necessary
    private Transform lokalTargetWaypoint;


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


        //-----------------------------------------------------------------------------------------------------------------
        //BEGIN OF LANE SWITCHING
        //-----------------------------------------------------------------------------------------------------------------

        //Timer to check if on right lane
        if (laneTimer <= 2f)
        {
            laneTimer += Time.deltaTime;
        }
        else
        {
            prepareLaneSwitch();
            laneTimer = 0f;
        }
    }

    //Check if laneSwitch is necessary to turn at next crossing and do so if thats the case
    private void prepareLaneSwitch()
    {
        //Bool to toggle looping
        laneLooping = true;

        //Check if 4 lane street
        if (travelRoute.Count >= 2 && nextLocalWaypoint.transform.parent.name.Contains("ShadowWaypoint"))
        {
            //Check which waypoint the car need to take to reach travelRoute[1] (basically, check if it needs to turn left or right)
            for (int x = 0; x < nexBigWaypoint.transform.childCount; x++)
            {
                if (laneLooping == true)
                {
                    foreach (GameObject waypoint in nexBigWaypoint.transform.GetChild(x).GetComponent<LocalWaypoint>()
                                 .connectedWaypoints)
                    {
                        if (waypoint.transform.parent == travelRoute[1])
                        {
                            //lokalTargetWaypoint is the lokalWaypoint the car needs to reach, so it can correctly turn at crossings
                            lokalTargetWaypoint = nexBigWaypoint.transform.GetChild(x);

                            //Check if current lane is the correct one to reach lokalTargetWaypoint, switch lane if not
                            Vector3 delta = (lokalTargetWaypoint.position - this.gameObject.transform.position);
                            if (Vector3.Cross(delta, this.gameObject.transform.right).y > 1
                                || Vector3.Cross(delta, this.gameObject.transform.right).y < -1)
                            {
                                switchLane();
                            }

                            laneLooping = false;
                            break;
                        }
                    }
                }
            }
        }
    }

    //Switch lane for ShadowWaypoints
    private void switchLane()
    {
        for (int x = 0; x < nextLocalWaypoint.transform.parent.childCount; x++)
        {
            if (nextLocalWaypoint.transform.parent.GetChild(x) == nextLocalWaypoint.transform)
            {
                //ShadowWaypoints have two children, the first [0] is the outside, the second [1] is the inside
                if (nextLocalWaypoint.transform.GetSiblingIndex() == 0)
                {
                    //Switch if next lane is empty
                    if (nextLocalWaypoint.transform.parent.GetChild(x + 1).gameObject.GetComponent<ShadowWaypoint>()
                            .carsOnLane == 0
                        && lastLocalWaypoint.transform.parent.GetChild(x + 1).gameObject.GetComponent<ShadowWaypoint>()
                            .carsOnLane == 0)
                    {
                        nextLocalWaypoint = nextLocalWaypoint.transform.parent.GetChild(x + 1).gameObject;
                        break;
                    }
                }
                else
                {
                    //Switch if next lane is empty
                    if (nextLocalWaypoint.transform.parent.GetChild(x - 1).gameObject.GetComponent<ShadowWaypoint>()
                            .carsOnLane == 0
                        && lastLocalWaypoint.transform.parent.GetChild(x - 1).gameObject.GetComponent<ShadowWaypoint>()
                            .carsOnLane == 0)
                    {
                        nextLocalWaypoint = nextLocalWaypoint.transform.parent.GetChild(x - 1).gameObject;
                        break;
                    }
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    //End OF LANE SWITCHING
    //-----------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        lastLocalWaypoint = origin.GetChild(0).gameObject;
        nextLocalWaypoint = lastLocalWaypoint.GetComponent<LocalWaypoint>().connectedWaypoints[0];
        myCarDetector = GetComponent<CarDetection>();
        carID = transform.root.GetComponent<Datenvisualisierung>().AddCarInDatenVisualisierung(GetComponent<MoveCar>());
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
            //Debug.Log($"Fährt von {lastWaypointIndex} über {nextWaypointIndex} zu {next2WaypointIndex} difference = {difference}");

            //...check if destination was reached and...
            if (travelRoute.Count == 0)
            {
                //...Despawn car
                gameManager.GetComponent<GameManager>().currentCars--;
                transform.root.GetComponent<Datenvisualisierung>().RemoveCarInDatenVisualisierung(GetComponent<MoveCar>());
                Destroy(this.gameObject);
            }
            //myCarDetector.SwitchLane();
        }

        //Rotate Object towards driving direction
        transform.LookAt(nextLocalWaypoint.transform);
        transform.Rotate(0, -90, 0);
    }

    GameObject getNextLocalWaypoint()
    {
        LocalWaypoint lastWaypoint = lastLocalWaypoint.GetComponent<LocalWaypoint>();
        nexBigWaypoint = travelRoute[0].gameObject;


        //Bool to toggle iteration foreach lopp
        bool toggleLoop = true;

        //Look maximum of three main Waypoints in advance to predict route to take in crossing (to switch lanes if necessary)
        Transform thirdNextWaypoint = null;
        if (travelRoute.Count > 2)
        {
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
                    for (int i = 0; i < nexBigWaypoint.transform.childCount; i++)
                    {
                        if (waypoint.transform == nexBigWaypoint.transform.GetChild(i).transform && toggleLoop == true)
                        {
                            nextLocalWaypoint = waypoint;
                            toggleLoop = false;
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
            speed -= (speed + (brakeDeceleration * Time.deltaTime)) * (left / (distanceToHaltelinie + right));
            // Stellen Sie sicher, dass die Geschwindigkeit nicht unter 0 fällt.
            speed = Mathf.Max(speed, 0);
        }
        else
        {
            //Debug.Log("Eigentliche warten, aber fährt nach rechts, deswegen egal");
        }
    }
}