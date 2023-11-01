using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LocalWaypoint : MonoBehaviour
{

    //List containing all connected lokal Waypoints
    public List<GameObject> connectedWaypoints = new List<GameObject>();

    //Draw Gizmos between this and all connected lokal Waypoints
    private void OnDrawGizmos()
    {
        foreach (GameObject waypoint in connectedWaypoints)
        {
            foreach (GameObject connectedWaypoint in connectedWaypoints) {

                if (connectedWaypoints.Count != 0 && connectedWaypoint != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, waypoint.transform.position);
                }
            }
            
        }
    }
}
