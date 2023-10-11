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
