using System.Collections;
using System.Collections.Generic;
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
         transform.position = Vector3.MoveTowards(transform.position, travelRoute[0].transform.position, speed * Time.deltaTime);
         transform.LookAt(travelRoute[0].transform);
         transform.Rotate(0, -90, 0);

         //If nextWaypoint was reached...
         if (Vector3.Distance(transform.position, travelRoute[0].transform.position) < 0.01) 
         {
             //Generate index of randomly chosen successor Waypoint
             //int randomWaypoint = Random.Range(0, travelRoute[0].GetComponent<Waypoint>().nextWaypoints.Count);

             //...check if Waypoint has successor, if not: Despawn car
             if (travelRoute[0].GetComponent<Waypoint>().nextWaypoints[0] == null)
             {
                 Destroy(this.gameObject);
             }

             //Else...
             else 
             {
                //...remove reached Waypoint
                travelRoute.Remove(travelRoute[0]);
             }
         }
    }
}
