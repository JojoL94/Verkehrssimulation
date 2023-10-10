using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Single Waypoint linked to the Waypoints directly behind and in front of
public class Waypoint : MonoBehaviour
{
    //List containing all connected previous Waypoints
    public List <Waypoint> previousWaypoints = new List<Waypoint>();
    //List containing all connected next Waypoints
    public List <Waypoint> nextWaypoints = new List<Waypoint>();

    //Starting drawing Gizsmos (Virsualizing the Waypoints in Unity Editor, while they stay invisible ingame)
    public void OnDrawGizmos()
    {
        //Give every Waypoint a blue WireSphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        //Draw a black line to connected previous Waypoints
        Gizmos.color = Color.black;

        Debug.Log(previousWaypoints.Count);
        if (previousWaypoints.Count > 0) {
            for (int i = 0; i < previousWaypoints.Count; i++)
            {
                if (previousWaypoints[i] != null)
                {
                    Gizmos.DrawLine(previousWaypoints[i].transform.position, transform.position);
                }
            }
        }
    }
}
