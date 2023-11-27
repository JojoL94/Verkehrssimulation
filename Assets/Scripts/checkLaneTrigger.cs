using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkLaneTrigger : MonoBehaviour
{
    //Draw Gizmo
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(1, 1, 1)); ;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if colliding object is car
        if (other.CompareTag("Car"))
        {
            //Check lane
            other.gameObject.GetComponent<MoveCar>().checkLaneSwitch();
        }
    }
}
