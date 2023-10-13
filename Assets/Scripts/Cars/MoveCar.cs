using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

//Script for Pathfinding and movement of cars
public class MoveCar : MonoBehaviour
{
    //Current destionation of car
    public Waypoint nextWaypoint;

    //Maximum speed of car
    public float maxSpeed = 1f;

    //Base acceleration speed of car in Unity units per second
    //It determines, along with Time.deltaTime, the increase of acceeration
    public float baseAcceleration = 5f;

    //Current speed of car
    public float speed = 0f;

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

    // Update is called once per frame
    void Update()
    {

        //Move to nextWaypoint
        transform.position = Vector3.MoveTowards(transform.position, nextWaypoint.transform.position, speed * Time.deltaTime);
        transform.LookAt(nextWaypoint.transform);

        //If nextWaypoint was reached...
        if (Vector3.Distance(transform.position, nextWaypoint.transform.position) < 0.01) 
        {
            //Generate index of randomly chosen successor Waypoint
            int randomWaypoint = Random.Range(0, nextWaypoint.GetComponent<Waypoint>().nextWaypoints.Count);

            //...check if Waypoint has successor, if not: Despawn car
            if (nextWaypoint.GetComponent<Waypoint>().nextWaypoints[0] == null)
            {
                Destroy(this.gameObject);
            }
            else 
            {
                //Otherwise, change current destination to a random successor
                nextWaypoint = nextWaypoint.GetComponent<Waypoint>().nextWaypoints[randomWaypoint];
            }
        }
    }
}
