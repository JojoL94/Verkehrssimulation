using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameManager is responsible for everything that's effecting the entire map (like max number of cars on map, etc.)
public class GameManager : MonoBehaviour
{
    //Max number of cars driving at the same time
    public float maxCars = 30;

    public Transform waypointCollection;
    public Transform streetCollection;

    // Start is called before the first frame update
    void Start()
    {

        int counter = 0;

        //In runtime put all Waypoints in waypoint list für A* Algorithm
        for (int x = 0; x < streetCollection.childCount; x++) {

            //Transform waypoint = streetCollection.GetChild(x).GetChild(transform.childCount);
            //waypoint.name = waypoint.name + "Nr. " + transform.childCount;

            while (streetCollection.GetChild(x).transform.childCount > 0)
            {
                streetCollection.GetChild(x).GetChild(transform.childCount).name = "Waypoint" + counter;
                streetCollection.GetChild(x).GetChild(transform.childCount).parent = waypointCollection.transform;
                counter++;
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
