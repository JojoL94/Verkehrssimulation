using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowWaypoint : MonoBehaviour
{
    //Number of cars currently on lane
    public int carsOnLane = 0;

    //If car enters collider, increase carsOnLane to check number of cars on lane
    private void OnTriggerEnter(Collider other)
    {
        carsOnLane++;
    }

    //If car enters collider, decrease carsOnLane to check number of cars on lane
    private void OnTriggerExit(Collider other)
    {
        carsOnLane--;
    }
}
