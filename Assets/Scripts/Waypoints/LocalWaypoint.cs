using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LocalWaypoint : MonoBehaviour
{

    //Max distance of Raycast
    public float RaycastDistance = 100f;

    //List containing all connected lokal Waypoints
    public List<GameObject> connectedWaypoints = new List<GameObject>();

    void Awake()
    {
        RaycastHit hit;

        //Connect all lokal Waypoints before start
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, RaycastDistance, LayerMask.GetMask("LokalWaypoint")))
        {
            this.connectedWaypoints.Add(hit.collider.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
