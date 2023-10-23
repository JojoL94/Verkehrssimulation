using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Single Waypoint linked to the Waypoints directly behind and in front of
public class Waypoint : MonoBehaviour
{
    //List containing all connected previous Waypoints
    //public List <Transform> previousWaypoints = new List<Transform>();
    
    //List containing all connected next Waypoints
    public List <Transform> neighbours = new List<Transform>();

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


        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i] != null)
            {

                Gizmos.DrawLine(neighbours[i].transform.position, transform.position);
            }
        }
      
    }

}
