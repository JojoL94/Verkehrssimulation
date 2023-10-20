using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Single Waypoint linked to the Waypoints directly behind and in front of
public class Waypoint : MonoBehaviour
{
    //List containing all connected previous Waypoints
    public List <Transform> previousWaypoints = new List<Transform>();
    
    //List containing all connected next Waypoints
    public List <Transform> nextWaypoints = new List<Transform>();

    //gCost => (distance of current Waypoint to start point)
    public float gCost;

    //hCost => (distance of current Waypoint to end point)
    public float hCost;

    //Starting drawing Gizsmos (Virsualizing the Waypoints in Unity Editor, while they stay invisible ingame)
    public void OnDrawGizmos()
    {
        //Give every Waypoint a blue WireSphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        //Draw a black line to connected previous Waypoints
        Gizmos.color = Color.black;


        if (previousWaypoints.Count > 0) {

            for (int i = 0; i < previousWaypoints.Count; i++)
            {
                if (previousWaypoints[i] != null)
                {

                    Gizmos.DrawLine(previousWaypoints[i].transform.position, transform.position);
                }
            }
        }


        //Draw a white line to connected next Waypoints
        Gizmos.color = Color.white;

        if (nextWaypoints.Count > 0)
        {
            for (int i = 0; i < nextWaypoints.Count; i++)
            {
                if (nextWaypoints[i] != null)
                {

                    Gizmos.DrawLine(nextWaypoints[i].transform.position, transform.position);
                }
            }
        }
    }

}
