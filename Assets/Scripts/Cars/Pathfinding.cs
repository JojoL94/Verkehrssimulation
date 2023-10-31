using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

//Script for Pathfinding of cars
public class Pathfinding : MonoBehaviour
{

    //GameObject containing hierarchy of waypoints to calculate route
    public GameObject waypointTree;


    //Retraces the shortest path from end point to start point
    void RetracePath(Transform startPoint, Transform endPoint){
        //List contains all Waypoints for path
        List<Transform> path = new List<Transform>();

        //Begin calculating path, once end was found
        Transform currentWaypoint = endPoint;

        //Retrace the path backwards, starting at the end
        while (currentWaypoint != startPoint) {
            path.Add(GameObject.Find("Waypoints").transform.Find(currentWaypoint.gameObject.name));
            currentWaypoint = currentWaypoint.parent;
        }
        //If done => reverse path, so that end point is at the end
        path.Reverse();

        //Add path to travelRoute in MoveCar Script, so that car starts to move the route
        this.GetComponent<MoveCar>().travelRoute = path;
        
    }





    //Calculate fastest route using A*Star Algorithm
    private void calculateRoute()
    {

        //Local copy of Waypoints collection => Bug fix to prevent each car from accessing the same collection
        waypointTree = Instantiate(GameObject.Find("Waypoints"));
        waypointTree.transform.parent = GameObject.Find("WaypointTreeLists").transform;
        waypointTree.transform.position = GameObject.Find("Waypoints").transform.position;

        //Spawn point of car
        Transform origin = this.GetComponent<MoveCar>().origin;

        //Local copy of spawn point
        origin = waypointTree.transform.Find(origin.name);

        //End point of car
        Transform destination = this.GetComponent<MoveCar>().destination;

        //Local copy of end point
        destination = waypointTree.transform.Find(destination.name);

        //List containing all Waypoints, that weren't checked so far
        List<Transform> openSet = new List<Transform>();

        //List containing all Waypoints that were checked
        List<Transform> closedSet = new List<Transform>();

        //First Waypoint is origin/spawn point
        openSet.Add(origin);


        while (openSet.Count > 0) {

            //currentWaypoint is first entry in openSet
            Transform currentWaypoint = openSet[0];


            //For every entry in openSet...
            for (int i = 1; i < openSet.Count; i++) {

                //..calculate f costs (gCost + hCost)...
                float fCostOpenSet = openSet[i].GetComponent<Waypoint>().gCost + openSet[i].GetComponent<Waypoint>().hCost;
                float fCostCurrent = currentWaypoint.GetComponent<Waypoint>().gCost + currentWaypoint.GetComponent<Waypoint>().hCost;

                //... and check distances
                //First compare fCost (sum of distances of Waypoint to start (gCost) and Waypoint to destination (hCost))
                //If fCost is equal => compare hCost (distance of Waypoint to destination)
                if (fCostOpenSet < fCostCurrent || 
                    (fCostOpenSet == fCostCurrent && openSet[i].GetComponent<Waypoint>().hCost < currentWaypoint.GetComponent<Waypoint>().hCost)) {
                    currentWaypoint = openSet[i];
                }
            }

            //Move currentWaypoint to closedSet
            openSet.Remove(currentWaypoint);
            closedSet.Add(currentWaypoint);

            //If destination was reached, retrace the path from destination to origin/ spawn point
            if (currentWaypoint == destination) {
                RetracePath(origin, destination);
                return;
            }

            //Check all neighbour Waypoints
            foreach (Transform neighbour in currentWaypoint.GetComponent<Waypoint>().neighbours) {
       
                //If neighbour Waypoint is in closedSet => was already checked and can be ignored
                if (closedSet.Contains(neighbour)) continue;

                //Else, calculate new gCosts (distance of current Waypoint to start point)
                float newGcost = currentWaypoint.GetComponent<Waypoint>().gCost + Vector3.Distance(currentWaypoint.localPosition, neighbour.localPosition);

                //If new path to neighbour Waypoint is shorter or neigbouring Waypoint is not in openSet...
                if (newGcost < neighbour.GetComponent<Waypoint>().gCost || !openSet.Contains(neighbour)) {

                    //...update gCost and hCost...
                    neighbour.GetComponent<Waypoint>().gCost = newGcost;
                    neighbour.GetComponent<Waypoint>().hCost = Vector3.Distance(neighbour.localPosition, destination.localPosition);

                    //...and set currentWaypoint as parent to neighbour Waypoint
                    neighbour.parent = currentWaypoint;

                    //Add neighbour Waypoint to openSet
                    //=> As openSet is filled again, continue the while loop from start
                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                    
                }
            }
        }

        //After calculation of route: Destroy local copy of Waypoints collection
        Destroy(this.gameObject.GetComponent<Pathfinding>().waypointTree);
    }

    private void Start()
    {
        //Calculate fastest route
        calculateRoute();
    }
}
