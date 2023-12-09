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
        MoveCar moveCar = other.gameObject.GetComponent<MoveCar>();
        //Check if colliding object is car
        if (other.CompareTag("Car") && (this.transform.parent.gameObject != moveCar.laneStreetObject))
        {
            //Save street Object of current trigger => prevents triggering neighbouring trigger in case of entering collider when switching
            moveCar.laneStreetObject = this.transform.parent.gameObject;
            //Check lane
            moveCar.checkLaneSwitch();
        }
    }
}
