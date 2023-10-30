using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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

    //Max distance of Raycast
    public float RaycastDistance = 100f;

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

    //Connect all Waypoints before start
    void Awake()
    {
        RaycastHit hit;

        //Connect all Main Waypoints
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, RaycastDistance, LayerMask.GetMask("MainWaypoint"))) {
            this.neighbours.Add(hit.collider.transform);
        }
    }

}
