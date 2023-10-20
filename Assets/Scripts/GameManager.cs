using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameManager is responsible for everything that's effecting the entire map (like max number of cars on map, etc.)
public class GameManager : MonoBehaviour
{
    //Max number of cars driving at the same time
    public float maxCars = 30;

    //List containing all Waypoints, acting as graph for A* Algorithm Pathfinding
    public List<Transform> graph = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        //waypointCollection is the folder/empty Gameobject which contains all Waypoints
        Transform waypointCollection = GameObject.Find("Waypoints").transform;

        //For every Waypoint in waypointCollection...
        for (int i = 0; i < waypointCollection.childCount; i++) {

            //...add to graph
            graph.Add(waypointCollection.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
