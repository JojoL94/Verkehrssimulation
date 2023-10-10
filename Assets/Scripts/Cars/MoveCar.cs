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

    //Current speed of car
    public float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        //Move to nextWaypoint
        transform.position = Vector3.MoveTowards(transform.position, nextWaypoint.transform.position, speed * Time.deltaTime);

        //If nextWaypoint was reached...
        if (Vector3.Distance(transform.position, nextWaypoint.transform.position) < 0.1) 
        {

            //...check if multiple destination Waypoint has multiple successors
            if (nextWaypoint.GetComponent<Waypoint>().nextWaypoints.Count > 1)
            {
                //If destination has multiple successors change current destination to a random successor
                nextWaypoint = nextWaypoint.GetComponent<Waypoint>().nextWaypoints[Random.Range(0, nextWaypoint.GetComponent<Waypoint>().nextWaypoints.Count)];
                transform.LookAt(nextWaypoint.transform);
            }
            //If destination Waypoint has one or less Waypoints...
            else
            {
                //...Check if Waypoint was defined, if not: Despawn car
                if (nextWaypoint.GetComponent<Waypoint>().nextWaypoints[0] == null)
                {
                    Destroy(this.gameObject);
                }
                //If Waypoint is defined, new Destination is the single successor
                else 
                {
                    nextWaypoint = nextWaypoint.GetComponent<Waypoint>().nextWaypoints[0];
                    transform.LookAt(nextWaypoint.transform);
                }
            }
        }
    }
}
