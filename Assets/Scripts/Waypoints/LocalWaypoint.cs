using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LocalWaypoint : MonoBehaviour
{

    public List<GameObject> connectedWaypoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        foreach (GameObject waypoint in connectedWaypoints)
        {
            if (connectedWaypoints.Count != 0 && connectedWaypoints[0] != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, waypoint.transform.position);
            }
        }
    }
}
