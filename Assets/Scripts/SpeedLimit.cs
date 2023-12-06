using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimit : MonoBehaviour
{
    public float speedLimit = 10f;

    private void OnTriggerEnter(Collider other)
    {
        //Check if colliding object is car
        if (other.CompareTag("Car"))
        {
            //Change maxSpeed
            other.gameObject.GetComponent<MoveCar>().maxSpeed = speedLimit;
        }
    }

    //Draw Gizmo
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(1, 1, 1)); ;
    }
}
