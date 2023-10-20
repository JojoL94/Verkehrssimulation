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
    }

    public void Start()
    {
        //Initialize costs for eacht Waypoint in nextWaypoints

        //Check if Waypoint has a predecessor
        /**if (previousWaypoints.Count > 0 && previousWaypoints[0] != null)
        {
            //For every Waypoint in previousWaypoints...
            for (int x = 0; x < previousWaypoints.Count; x++)
            {
                //...calculate gCost based on distance to neigbour in previousWaypoints 
                float totalCost = Vector3.Distance(this.transform.localPosition, previousWaypoints[x].localPosition);

                //... and add it to list
                gCostPrevious.Add(totalCost);
            }
        }

        //Check if Waypoint has a successor
        if (nextWaypoints.Count > 0 && nextWaypoints[0] != null) {

            //For every Waypoint in nextWaypoints...
            for (int x = 0; x < nextWaypoints.Count; x++)
            {
                //...calculate gCost based on distance to neigbour in nextWaypoints...
                float totalCost = Vector3.Distance(this.transform.localPosition, nextWaypoints[x].localPosition);

                //... and add it to list
                gCostNext.Add(totalCost);
            }
        }*/
    }
}
