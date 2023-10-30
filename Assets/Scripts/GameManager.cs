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

    //Counter to give every Main Waypoint an uniqe name
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {

        //In runtime put all Waypoints in waypoint list für A* Algorithm
        for (int x = 0; x < streetCollection.childCount; x++) {

                //Filter out Right before Left Collider
                while (streetCollection.GetChild(x).transform.childCount > 0 && !streetCollection.GetChild(x).GetChild(transform.childCount).name.Contains("RightOfWay"))
                {
                    //Name each Waypoint with uniqe name and put in  waypointCollection(A* Algorithm needs Waypoints with uniqe names and in separate List)
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
