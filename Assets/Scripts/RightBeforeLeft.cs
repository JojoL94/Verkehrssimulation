using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightBeforeLeft : MonoBehaviour
{

    public Transform lookTrigger;
    public Transform sendCars;

    // Die Richtung des Raycasts (in diesem Fall in die Vorwärtsrichtung des GameObjects)
    Vector3 raycastDirection;

    // Die Länge des Raycasts
    float raycastDistance = 8f;

    // Führe den eigentlichen Raycast durch
    RaycastHit hitSend;
    RaycastHit hitLook;

    public enum Behaviour { LookForOtherCar, SendCar };
    public Behaviour behaviour;

    public GameObject lookCar, sendCar;
    public bool lookHasCollision = false;
    public bool sendHasCollision = false;

    public float left, right;

    public LayerMask layerMask;
    private void Start()
    { 
        // Die Richtung des Raycasts (in diesem Fall in die Vorwärtsrichtung des GameObjects)
        raycastDirection = -transform.forward;
    }

    void Update()
    {
        Debug.DrawRay(sendCars.position, sendCars.TransformDirection(Vector3.back) * raycastDistance, Color.red);
        if (Physics.Raycast(sendCars.position, sendCars.TransformDirection(Vector3.back), out hitSend, raycastDistance, layerMask))
        {                                                                                         // Überprüfen, ob das getroffene Objekt ein Auto ist
            //Debug.DrawRay(sendCars.position, sendCars.TransformDirection(Vector3.back) * raycastDistance, Color.cyan); // Zeichne den Raycast in der Szene
            sendCar = hitSend.collider.gameObject.transform.parent.gameObject;
            if(sendCar!=lookCar)
                sendHasCollision = true;
            else
            {
                sendHasCollision = false;
                sendCar = null;
            }
        }
        else
        {
            sendHasCollision = false;
            sendCar = null;
        }

        Debug.DrawRay(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back) * raycastDistance / 2, Color.green);
        if (Physics.Raycast(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back), out hitLook, raycastDistance/2, layerMask))
        {                                                                                         // Überprüfen, ob das getroffene Objekt ein Auto ist
            //Debug.DrawRay(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back) * raycastDistance, Color.magenta); // Zeichne den Raycast in der Szene
            lookCar = hitLook.collider.gameObject.transform.parent.gameObject;
            if (sendCar != lookCar)
                lookHasCollision = true;
            else
            {
                lookHasCollision = false;
                lookCar = null;
            }
        }
        else
        {
            lookHasCollision = false;
            lookCar = null;
        }

        //If look (Right before Left trigger)
        if (lookCar != null) {
            if (lookCar.GetComponent<MoveCar>().look != null)
            {
                MoveCar lookMoveCar = lookCar.GetComponent<MoveCar>();
                //Check Distance, if Distance small => car is in crossing, ignore right before left so it does not block the corssing
                if (Vector3.Distance(lookMoveCar.transform.position, this.transform.position) < 1f)
                {
                    lookMoveCar.isInIntersection = true;
                }
            }
        }

        if (sendHasCollision && lookHasCollision && (lookCar != sendCar && !lookCar.transform.IsChildOf(sendCar.transform) && !sendCar.transform.IsChildOf(lookCar.transform)))
            if(sendCar != null && lookCar != null)
            {
                if (lookCar.GetComponent<MoveCar>() != null && lookCar.GetComponent<MoveCar>().isInIntersection == false)
                {
                    lookCar.GetComponent<MoveCar>().giveWait(Vector3.Distance(transform.position, lookCar.transform.position)/2, left, right, causingBrake: "RechtsVorLinks");
                }
            }
    }

    void OnDrawGizmos()
    {
        // Zeichne den Raycast mit dem Offset im Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(sendCars.position, sendCars.TransformDirection(Vector3.back) * raycastDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back) * raycastDistance / 2);
    }

}
