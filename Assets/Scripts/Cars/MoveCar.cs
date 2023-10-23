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



    //Fixed Update is used for physics calculations that aren't linear
    private void FixedUpdate()
    {
        //Acceleration
        if (speed < maxSpeed)
        {
            //In brackets: Calculate the acceleration of speed
            //Multiply result by Time.deltaTime to get acceleration per seconds
            speed += (speed + (Time.deltaTime * baseAcceleration)) * Time.deltaTime;

            //Ensure, that speed is never bigger than maxSpeed
            speed = Mathf.Clamp(speed, 0, maxSpeed);
        }
    }

    private void Start()
    {
        lastLocalWaypoint = origin.GetChild(0).gameObject;
    }



    // Update is called once per frame
    void Update()
    {
        getNextLocalWaypoint();
        //Move to neighbouring Waypoint
        transform.position = Vector3.MoveTowards(transform.position, nextLocalWaypoint.transform.position, speed * Time.deltaTime);
        transform.LookAt(nextLocalWaypoint.transform);
        transform.Rotate(0, -90, 0);

        //If neighbouring Waypoint was reached...
        if (Vector3.Distance(transform.position, nextLocalWaypoint.transform.position) < 0.01)
        {
            //...remove reached Waypoint and...
            lastLocalWaypoint = nextLocalWaypoint;
            travelRoute.Remove(travelRoute[0]);

            //...check if destination was reached and...
            if (travelRoute.Count == 0) {

                //...Despawn car...
                Destroy(this.gameObject);

                //...and destroy local copy of Waypoints collection
                Destroy(this.gameObject.GetComponent<Pathfinding>().waypointTree);
            }
        }
    }


    public GameObject nexBigWaypoint;
    GameObject getNextLocalWaypoint()
    {
        LocalWaypoint lastWaypoint = lastLocalWaypoint.GetComponent<LocalWaypoint>();
        nexBigWaypoint = travelRoute[0].gameObject;

        foreach(GameObject waypoint in lastWaypoint.connectedWaypoints)
        {
            Debug.Log("A " +waypoint.gameObject.name);
            if(waypoint!=null)
                if(nexBigWaypoint!=null)
                    for(int i = 0; i < nexBigWaypoint.transform.childCount; i++)
                    {
                        Debug.Log("B " + nexBigWaypoint.transform.GetChild(i).gameObject.name);
                        if (waypoint.transform == nexBigWaypoint.transform.GetChild(i).transform)
                        {
                            nextLocalWaypoint = waypoint;
                        }
                    }
        }
        return null;
    }
}
