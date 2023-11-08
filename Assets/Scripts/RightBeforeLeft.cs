using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightBeforeLeft : MonoBehaviour
{

    public Transform lookTrigger;
    public Transform sendCars;

    // Die Richtung des Raycasts (in diesem Fall in die Vorw�rtsrichtung des GameObjects)
    Vector3 raycastDirection;

    // Die L�nge des Raycasts
    float raycastDistance = 6.75f;

    // F�hre den eigentlichen Raycast durch
    RaycastHit hitSend;
    RaycastHit hitLook;

    public enum Behaviour { LookForOtherCar, SendCar };
    public Behaviour behaviour;


    public bool lookHasCollision = false;
    public bool sendHasCollision = false;

    public float left, right;

    public LayerMask layerMask;
    private void Start()
    { 
        // Die Richtung des Raycasts (in diesem Fall in die Vorw�rtsrichtung des GameObjects)
        raycastDirection = -transform.forward;
    }

    void Update()
    {
        if (Physics.Raycast(sendCars.position, sendCars.TransformDirection(Vector3.back), out hitSend, raycastDistance, layerMask))
        {                                                                                         // �berpr�fen, ob das getroffene Objekt ein Auto ist
            //Debug.DrawRay(sendCars.position, sendCars.TransformDirection(Vector3.back) * raycastDistance, Color.cyan); // Zeichne den Raycast in der Szene
            sendHasCollision = true;
        }
        else sendHasCollision = false;

        if (Physics.Raycast(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back), out hitLook, raycastDistance, layerMask))
        {                                                                                         // �berpr�fen, ob das getroffene Objekt ein Auto ist
            //Debug.DrawRay(lookTrigger.position, lookTrigger.TransformDirection(Vector3.back) * raycastDistance, Color.magenta); // Zeichne den Raycast in der Szene
            lookHasCollision = true;
        }
        else lookHasCollision = false;

        /*Vector3 newPos2 = new Vector3(lookTrigger.position.x + 1, lookTrigger.position.y, lookTrigger.position.z);
        if (Physics.Raycast(newPos2, lookTrigger.TransformDirection(Vector3.back), out hitLook, raycastDistance, layerMask))
        {                                                                                         // �berpr�fen, ob das getroffene Objekt ein Auto ist
            //Debug.DrawRay(newPos2, lookTrigger.TransformDirection(Vector3.back) * raycastDistance, Color.magenta); // Zeichne den Raycast in der Szene
            lookHasCollision = true;
        }
        else lookHasCollision = false;

        Vector3 newPos3 = new Vector3(lookTrigger.position.x - 1, lookTrigger.position.y, lookTrigger.position.z);
        if (Physics.Raycast(newPos3, lookTrigger.TransformDirection(Vector3.back), out hitLook, raycastDistance, layerMask))
        {                                                                                         // �berpr�fen, ob das getroffene Objekt ein Auto ist
            //Debug.DrawRay(newPos3, lookTrigger.TransformDirection(Vector3.back) * raycastDistance, Color.magenta); // Zeichne den Raycast in der Szene
            lookHasCollision = true;
        }
        else lookHasCollision = false;*/

        if (sendHasCollision && lookHasCollision)
            if (hitLook.collider.GetComponent<MoveCar>())
                hitLook.collider.GetComponent<MoveCar>().giveWait(Vector3.Distance(transform.position, hitLook.collider.transform.position), left, right);
            else if (hitLook.collider.transform.parent.GetComponent<MoveCar>())
                hitLook.collider.transform.parent.GetComponent<MoveCar>().giveWait(Vector3.Distance(transform.position, hitLook.collider.transform.position), left, right);
    }

}
