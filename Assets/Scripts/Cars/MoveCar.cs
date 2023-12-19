using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


//Script for movement of cars
public class MoveCar : MonoBehaviour
{
    //Identification Variable
    public int carID;
    //PrefabID
    public int prefabId;
    //Spawn point of car
    public Transform origin;
    //End point of car
    public Transform destination;
    //List containing all Waypoints the car takes to reach destination
    public List<Transform> travelRoute = new List<Transform>();
    //Maximum speed of car
    public float maxSpeed = 6f;
    // Average speed
    public float averageSpeed;
    public float maxSpeedOffset;
    //Base acceleration speed of car in Unity units per second
    //It determines, along with Time.deltaTime, the increase of acceeration
    private float baseAcceleration = 100f;
    //Current speed of car
    public float speed = 0f;
    public GameObject lastLocalWaypoint, nextLocalWaypoint, next2LocalWaypoint;
    //GameManager to collect information effecting the whole level
    public GameObject gameManager;
    //nexBigWaypoint saves the next mainWaypoint
    public GameObject nexBigWaypoint;
    //Variables for Braking
    public bool doBrake = false;
    private float brakeDeceleration;
    public float brakeDecelerationModifier;
    private float timer;
    public float brakeTimer = 3f;
    //Variables for rotation
    private float rotationSpeed = 5f;
    // Need to give wait
    public bool doGiveWait = false;
    //Variables for Distance
    private CarDetection myCarDetector;
    //Timer to check time needed to reach waypoint
    private float waypointTime;
    //Timer to determine when to recheck pathfinding => to drive around traffic jams
    private float pathfindingTimer;
    //Bool to toggle for- and foreach-loops in lane switching
    private bool laneLooping = true;
    //Temporarily saves parent of last laneSwitchTrigger to prevent switching into another trigger of same parent
    public GameObject laneStreetObject;
    //lokalTargetWaypoint saves the child of the next mainWaypoint => needs to be reached to turn left/right if necessary
    public Transform lokalTargetWaypoint;
    // Is the car turning right? If so don't give wait
    public bool turnsRight;
    //Containts the initial y value to offset different heights of preFab => Used to fix "some cars sink in road"-bug
    public float initalYValue = 0f;
    // Transform to get information if car is blocking the intersection
    Transform n2LocalWaypointSend;
    //Position of nextLocalWaypoint
    Vector3 nextLocalWaypointPosition = new Vector3();
    public string causingBrake = "";
    public GameObject objectInIntersection = null;
    

    //Variables for Types of Driver
    private int drivingType;

    public int aggressivenessLevel;
    
    //Fixed Update is used for physics calculations that aren't linear
    private void FixedUpdate()
    {
        //Acceleration
        if (speed < maxSpeed && !doBrake)
        {
            //In brackets: Calculate the acceleration of speed
            //Multiply result by Time.deltaTime to get acceleration per seconds
            speed += (speed + (Time.deltaTime * baseAcceleration)) * Time.deltaTime;

            float tmpMaxSpeed = maxSpeed - maxSpeedOffset;
            //Ensure, that speed is never bigger than maxSpeed
            speed = Mathf.Clamp(speed, 0, tmpMaxSpeed);
        }

        //Deceleration
        if (speed > maxSpeed && !doBrake)
        {
            doBrake = true;
        }



        if (doBrake)
        {
            timer += Time.deltaTime;
            if (timer >= brakeTimer)
            {
                doBrake = false;
                timer = 0f;
            }

            float tmpBrakeDeceleration = brakeDeceleration + (brakeDeceleration * brakeDecelerationModifier/100);
            // Verringere die Geschwindigkeit basierend auf der Bremsdeceleration
            speed -= (speed +(tmpBrakeDeceleration * Time.deltaTime)) * Time.deltaTime;

            // Stelle sicher, dass die Geschwindigkeit nicht negativ wird (rückwärts fahren)
            speed = Mathf.Max(speed, 0f);
        }
        else
        {
            timer = 0f;
        }

    }

    //-----------------------------------------------------------------------------------------------------------------
    //BEGIN OF LANE SWITCHING
    //-----------------------------------------------------------------------------------------------------------------

    //Check if laneSwitch is necessary to turn at next crossing and do so if thats the case
    public void checkLaneSwitch()
    {
        //Bool to toggle looping
        laneLooping = true;
        lokalTargetWaypoint = null;

        //Check if more than 2 big waypoints to prevent null pointer
        if (travelRoute.Count >= 2 && nextLocalWaypoint.transform.parent.name.Contains("ShadowWaypoint"))
        {
            //Check which waypoint the car need to take to reach travelRoute[1] (basically, check if it needs to turn left or right)
            //First, check if current lane doesn't need change
            for (int x = 0; x < nexBigWaypoint.transform.childCount; x++)
            {
                if (laneLooping == true)
                {
                    foreach (GameObject waypoint in nexBigWaypoint.transform.GetChild(x).GetComponent<LocalWaypoint>()
                                 .connectedWaypoints)
                    {
                        Vector3 delta1 = (nexBigWaypoint.transform.GetChild(x).transform.position - this.gameObject.transform.position).normalized;
                        if (Vector3.Cross(delta1, this.gameObject.transform.right).y < 0.08 &&
                            Vector3.Cross(delta1, this.gameObject.transform.right).y > -0.15)
                        {
                            nexBigWaypoint.transform.GetChild(x);
                            if (waypoint.transform.parent == travelRoute[1])
                            {
                                //lokalTargetWaypoint is the lokalWaypoint the car needs to reach, so it can correctly turn at crossings
                                lokalTargetWaypoint = nexBigWaypoint.transform.GetChild(x);
                                laneLooping = false;
                                break;
                            }
                        }

                    }
                }
            }
            //Check which waypoint the car need to take to reach travelRoute[1] (basically, check if it needs to turn left or right)
            if (lokalTargetWaypoint == null) { 
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
                                laneLooping = false;
                                break;
                            }
                        }
                    }
                }
            }
            //Check if current lane is the correct one to reach lokalTargetWaypoint, switch lane if not
            Vector3 delta = (lokalTargetWaypoint.position - this.gameObject.transform.position).normalized;
            //y > 0 = car is on the right side compared to the targetWaypoint => needs to switch to left lane
            // y < 0 = car is on the left side compared to the targetWaypoint => needs to switch to right lane
            // y == 0 = car is exactly in front of targetWaypoint => needs no lane switch
            //ATTENTION: Our waypoints aren't properly alligned and probably never will be, because of different PreFabs
            // => Because of that we can't use exactly 0 as comparison
            if (Vector3.Cross(delta, this.gameObject.transform.right).y > 0.08
                || Vector3.Cross(delta, this.gameObject.transform.right).y < -0.15)
            {
                switchLane();

            }
        }
    }

    //Switch lane for ShadowWaypoints
    public void switchLane()
    {
        //Check if 4 lane street
        if (nextLocalWaypoint.transform.parent.name.Contains("ShadowWaypoint")) {
            for (int x = 0; x < nextLocalWaypoint.transform.parent.childCount; x++)
            {
                if (nextLocalWaypoint.transform.parent.GetChild(x) == nextLocalWaypoint.transform)
                {
                    //ShadowWaypoints have two children, the first [0] is the outside, the second [1] is the inside
                    if (nextLocalWaypoint.transform.GetSiblingIndex() == 0)
                    {
                        nextLocalWaypoint = nextLocalWaypoint.transform.parent.GetChild(x + 1).gameObject;
                        break;
                    }
                    else
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
        brakeDeceleration = baseAcceleration * 2;
        StartCoroutine(test());
        // Start Cortourine for calculating averageSpeed
        StartCoroutine(CalculateAvgSpeed());
        get2ndNextWaypoint();
        //Setup Driver Type (16 different Driver Types possible)
        bool tmpIntSetupDrivingType  = (Random.value > 0.5f);
        //int tmpIntSetupDrivingType = Random.Range(0, 1);
        if (!tmpIntSetupDrivingType)
        {
            //maxSpeedOffset = Random.Range(maxSpeed/4, maxSpeed/3);  //Langsam
            maxSpeedOffset = maxSpeed / 3;
        }
        else
        {
            //maxSpeedOffset = Random.Range(0, maxSpeed/4); //Schnell
            maxSpeedOffset = 0;
            aggressivenessLevel++;
        }
        tmpIntSetupDrivingType  = (Random.value > 0.5f);
        if (!tmpIntSetupDrivingType)
        {
            //baseAcceleration -= Random.Range(baseAcceleration/4, baseAcceleration/3); //Langsam
            baseAcceleration -= baseAcceleration / 3;
        }
        else
        {
            aggressivenessLevel++;
            //baseAcceleration -= Random.Range(0, baseAcceleration/4); //Schnell
        }
        tmpIntSetupDrivingType  = (Random.value > 0.5f);
        if (!tmpIntSetupDrivingType)
        {
            aggressivenessLevel++;
            //brakeDeceleration += Random.Range(brakeDeceleration/4, brakeDeceleration/3); //Harter Bremser
            brakeDeceleration += brakeDeceleration / 3;
        }
        else
        {
            //brakeDeceleration += Random.Range(0, brakeDeceleration/4); //weicher Bremser
            brakeDeceleration += 0;
        }
        tmpIntSetupDrivingType  = (Random.value > 0.5f);
        if (!tmpIntSetupDrivingType)
        {
            //myCarDetector.minTargetDistance += Random.Range(myCarDetector.minTargetDistance/4, myCarDetector.minTargetDistance/3); //großer Abstand
            myCarDetector.minTargetDistance += myCarDetector.minTargetDistance / 3;
        }
        else
        {
            aggressivenessLevel++;
            //myCarDetector.minTargetDistance += Random.Range(0, myCarDetector.minTargetDistance/4); //kleiner Abstand
            
        }
        //End Setup Driver Type
    }

    // Update is called once per frame
    void Update()
    {
        //Timer to check time needed to reach waypoint
        waypointTime += Time.deltaTime;

        //Timer to re-check pathfinding
        pathfindingTimer += Time.deltaTime;

        //Timer for new pathfinding activates every 10 seconds
        if (pathfindingTimer == 10f) {
            //Look 4 Waypoints ahead, to detect traffic jams
            if (travelRoute[3] != null) {
                //If timecost bigger than 17.5 => traffic jam => new pathfinding
                if (travelRoute[3].GetComponent<Waypoint>().timeCost > 17.5) {
                    this.GetComponent<Pathfinding>().calculateRoute();
                }
            }
            pathfindingTimer = 0f;
        }

        //Current fix of "cars stuck in road"-bug:
        //change y value of affected cars (car4 and car5) in preFab and add preFab-y value on top of waypoint y-value
        nextLocalWaypointPosition = new Vector3(nextLocalWaypoint.transform.position.x, nextLocalWaypoint.transform.position.y + initalYValue, nextLocalWaypoint.transform.position.z);

        //Move to neighbouring Waypoint
        transform.position = Vector3.MoveTowards(transform.position, nextLocalWaypointPosition,
            speed * Time.deltaTime);
        // If car is getting closer to intersection
        if (Vector3.Distance(transform.position, nextLocalWaypointPosition) <= 4.5f)
        {
            // Check if intersection is clean
            if (n2LocalWaypointSend != null)
            {
                SendIntersection sendIntersection = n2LocalWaypointSend.GetComponent<SendIntersection>();
                if (sendIntersection != null)
                {
                    
                    if (sendIntersection.hasCollision && transform.name != sendIntersection.hittingCar.name)
                    {
                        objectInIntersection = sendIntersection.hittingCar;
                        // If something is blocking the intersectiong, wait;
                        //Debug.Log($"{name} gives wait --> Intersection is blocked by {objectInIntersection}");
                        giveWait(Vector3.Distance(transform.position, nextLocalWaypointPosition)/10,causingBrake:"intersectionBlocked");
                        return;
                    }

                }
            }
        }

        if (Vector3.Distance(transform.position, nextLocalWaypointPosition) < 0.5)
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
        // Bestimme die Richtung zum Ziel-Waypoint
        Vector3 targetDirection = (nextLocalWaypointPosition - transform.position).normalized;

        // Berechne die Rotation, um die Zielrichtung zu erreichen
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion offSetTargetRotation = Quaternion.Euler(0f, -90f, 0f);

        //Rotate Object towards driving direction
        // Führe eine lineare Interpolation zwischen der aktuellen Rotation und der Zielrotation durch
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation * offSetTargetRotation, Time.deltaTime * rotationSpeed);

        //transform.LookAt(nextLocalWaypoint.transform);
        //transform.Rotate(0, -90, 0);
    }

    IEnumerator test()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            causingBrake = "";
        }
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
            //Update timeCost
            travelRoute[0].gameObject.GetComponent<Waypoint>().timeCost = waypointTime;
            waypointTime = 0f;

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

        get2ndNextWaypoint();

        return null;
    }

    void get2ndNextWaypoint()
    {
        if (travelRoute.Count > 1)
            if (travelRoute[1] != null)
            {
                next2LocalWaypoint = nextLocalWaypoint.GetComponent<LocalWaypoint>().connectedWaypoints[0];

                foreach (GameObject localWaypoint in nextLocalWaypoint.GetComponent<LocalWaypoint>().connectedWaypoints)
                {
                    if (localWaypoint.transform.parent == travelRoute[1].transform)
                        next2LocalWaypoint = localWaypoint.GetComponent<LocalWaypoint>().gameObject;
                }
            }

        if (next2LocalWaypoint.transform.Find("Send") != null)
            n2LocalWaypointSend = next2LocalWaypoint.transform.Find("Send");
        else n2LocalWaypointSend = null;
    }


    // Is called by intersection and its BoxColliders
    public void giveWait(float distanceToHaltelinie = .1f, float left = 0.3f, float right = 12f, string causingBrake = "")
    {
        this.causingBrake = causingBrake;
        if (!turnsRight)
        {
            // Je näher das Auto an der Haltelinie ist, desto stärker wird gebremst
            float normalizedDistance = Mathf.Clamp01(distanceToHaltelinie / 10f);
            float test = 17f * (1 - normalizedDistance);
            doBrake = true;
            // Breaking is harder, the closer to the Haltelinie, the harder breake.
            // speed -= (speed + (brakeDeceleration * Time.deltaTime)) * (left / (distanceToHaltelinie + right));

            speed -= test * Time.deltaTime;
            // Stellen Sie sicher, dass die Geschwindigkeit nicht unter 0 fällt.
            speed = Mathf.Max(speed, 0);
        }
        else
        {
            doBrake = false;
            //Debug.Log("Eigentliche warten, aber fährt nach rechts, deswegen egal");
        }
    }

    private void OnMouseDown()
    {
        CarInfoView.instance.ChangeCarInfoTo(this);
    }

    
    IEnumerator CalculateAvgSpeed()
    {
        int counter = 0;
        float sumSpeed = 0;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            counter++;
            sumSpeed += speed;
            averageSpeed = sumSpeed/counter;
        }
    }
}