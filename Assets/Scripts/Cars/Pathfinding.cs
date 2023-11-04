using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static Pathfinding;

//Script for Pathfinding of cars
public class Pathfinding : MonoBehaviour
{
    //New class WaypointTree for nested hierachy of waypoints, needed for A* algorithm
    public class WaypointTree
    {
        public WaypointTree parentWaypointObject;
        public int numberParents;
        public Transform id;
    }

    //WaypointTree contains sorted hiearchie of all Waypoints by A* algorithm
    WaypointTree waypointTreeList = new WaypointTree();

    //WaypointTree contains WaypointTree object of destination to later retrace the path backwards
    WaypointTree destinationTree = new WaypointTree();

    //WaitingList to connect parents of WaypointTree objects
    List<WaypointTree> waitingList = new List <WaypointTree>();

    //Retraces the shortest path from end point to start point
    void RetracePath(Transform startPoint){

        //List contains all Waypoints for path
        List<Transform> path = new List<Transform>();

        //Begin calculating path, once end was found
        WaypointTree currentWaypoint = destinationTree;

        //Retrace the path backwards, starting at the end
        while (currentWaypoint.id != startPoint) {
            path.Add(GameObject.Find("Waypoints").transform.Find(currentWaypoint.id.gameObject.name));
            currentWaypoint = currentWaypoint.parentWaypointObject;
         }

        //If done => reverse path, so that end point is at the end
        path.Reverse();

        //Add path to travelRoute in MoveCar Script, so that car starts to move the route
        this.GetComponent<MoveCar>().travelRoute = path;

    }


    //Calculate fastest route using A*Star Algorithm
    private void calculateRoute()
    {
        //Spawn point of car
        Transform origin = this.GetComponent<MoveCar>().origin;


        //End point of car
        Transform destination = this.GetComponent<MoveCar>().destination;

        //List containing all Waypoints, that weren't checked so far
        List<Transform> openSet = new List<Transform>();

        //List containing all Waypoints that were checked
        List<Transform> closedSet = new List<Transform>();

        //First Waypoint is origin/spawn point
        openSet.Add(origin);

        //Fill waitingList with origin
        waitingList.Add(new WaypointTree() {id = origin });

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
                RetracePath(origin);
                return;
            }

            //Check all neighbour Waypoints
            foreach (Transform neighbour in currentWaypoint.GetComponent<Waypoint>().neighbours) {

                //Temporary WaypointTree object to contain current neigbour
                WaypointTree child = new WaypointTree();
              
                //If neighbour Waypoint is in closedSet => was already checked and can be ignored
                if (closedSet.Contains(neighbour)) continue;

                //Else, calculate new gCosts (distance of current Waypoint to start point)
                float newGcost = currentWaypoint.GetComponent<Waypoint>().gCost + Vector3.Distance(currentWaypoint.localPosition, neighbour.localPosition);

                //If new path to neighbour Waypoint is shorter or neigbouring Waypoint is not in openSet...
                if (newGcost < neighbour.GetComponent<Waypoint>().gCost || !openSet.Contains(neighbour)) {

                    //...update gCost and hCost
                    neighbour.GetComponent<Waypoint>().gCost = newGcost;
                    neighbour.GetComponent<Waypoint>().hCost = Vector3.Distance(neighbour.localPosition, destination.localPosition);

                    //Set currentWaypoint as id
                    child.id = neighbour;

                    //Get number of connected neighbours
                    child.numberParents = child.id.gameObject.GetComponent<Waypoint>().neighbours.Count;

                    //Add to waiting list to process later
                    waitingList.Add(child);

                    //Check if an entry in waitingList matches currently searched parent
                    foreach (WaypointTree waypoint in waitingList)
                    {
                        //If so => Set entry as parent and remove from list
                        if (currentWaypoint == waypoint.id)
                        {
                            child.parentWaypointObject = waypoint;

                            waypoint.numberParents--;

                            if (waypoint.numberParents <= 1)
                            {
                                //Debug.Log(waypoint.id);
                                waitingList.Remove(waypoint);
                            }

                            break;
                        }
                    }

                    //If WaypointTree object is destionation, save in destinationTree to use it later in retracePath
                    if (child.id == destination)
                    {
                        destinationTree = child;
                    }

                    //Add too root object
                    waypointTreeList = child;

                    //Add neighbour Waypoint to openSet
                    //=> As openSet is filled again, continue the while loop from start
                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                  
                }
            }
        }
    }

    private void Start()
    {
        //Calculate fastest route
        calculateRoute();
    }
}
