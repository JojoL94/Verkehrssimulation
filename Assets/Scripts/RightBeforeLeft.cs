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
    float raycastDistance = 6.75f;

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
        Debug.DrawRay(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back) * raycastDistance, Color.white);
        if (Physics.Raycast(sendCars.position, sendCars.TransformDirection(Vector3.back), out hitSend, raycastDistance, layerMask))
        {                                                                                         // Überprüfen, ob das getroffene Objekt ein Auto ist
            //Debug.DrawRay(sendCars.position, sendCars.TransformDirection(Vector3.back) * raycastDistance, Color.cyan); // Zeichne den Raycast in der Szene
            sendHasCollision = true;
            sendCar = hitSend.collider.gameObject;
        }
        else
        {
            sendHasCollision = false;
            sendCar = null;
        }

        Debug.DrawRay(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back) * raycastDistance, Color.gray);
        if (Physics.Raycast(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back), out hitLook, raycastDistance, layerMask))
        {                                                                                         // Überprüfen, ob das getroffene Objekt ein Auto ist
            //Debug.DrawRay(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back) * raycastDistance, Color.magenta); // Zeichne den Raycast in der Szene
            lookHasCollision = true;
            lookCar = hitLook.collider.gameObject;
        }
        else
        {
           lookHasCollision = false;
            lookCar = null;
        }
        if (sendHasCollision && lookHasCollision && (lookCar!=sendCar && lookCar.transform.IsChildOf(sendCar.transform) && sendCar.transform.IsChildOf(lookCar.transform)))
            if (hitLook.collider.GetComponent<MoveCar>())
                hitLook.collider.GetComponent<MoveCar>().giveWait(Vector3.Distance(transform.position, hitLook.collider.transform.position), left, right, causingBrake: "RechtsVorLinks");
            else if (hitLook.collider.transform.parent.GetComponent<MoveCar>())
                hitLook.collider.transform.parent.GetComponent<MoveCar>().giveWait(Vector3.Distance(transform.position, hitLook.collider.transform.position), left, right, causingBrake: "RechtsVorLinks");
    }

}
