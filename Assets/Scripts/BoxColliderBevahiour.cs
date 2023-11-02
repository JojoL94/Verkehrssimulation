using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderBevahiour : MonoBehaviour
{

    // Determines in Editor(Inspector) if Collider should look or send
    public enum Behaviour { LookForOtherCar, SendCar};
    public Behaviour behaviour;

    // If behaviour == lookForOtherCar, lookAt determines which boxCollide it should look at
    public BoxColliderBevahiour lookAt;

    // Has Collision?
    public bool hasCollision;

    private void OnTriggerEnter(Collider other)
    {
        // Only the "nose" of the car should look --> Don't care if only booty is in trigger
        if (other.CompareTag("CarLook"))
        {
            //Debug.Log($"Enter! {other.gameObject.name}");
            hasCollision = true;
            if (behaviour == Behaviour.LookForOtherCar)
            {
                //Debug.Log($"Give wait! {other.gameObject.name}");

                // Check if a car is in other BoxCollider
                if (lookAt.hasCollision)
                    // Tell the car to brake, cuz has to give wait
                    other.transform.parent.GetComponent<MoveCar>().giveWait(Vector3.Distance(other.transform.position, transform.GetChild(0).position));
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (behaviour == Behaviour.LookForOtherCar)
        {
            // Only the "nose" of the car should look --> Don't care if only booty is in trigger
            if (other.CompareTag("CarLook"))
            {
                //Debug.Log($"Stay! {other.gameObject.name}");
                hasCollision = true;
                //Debug.Log($"Give wait! {other.gameObject.name}");

                // Check if a car is in other BoxCollider
                if (lookAt.hasCollision)
                    // Tell the car to brake, cuz has to give wait
                    other.transform.parent.GetComponent<MoveCar>().giveWait(Vector3.Distance(other.transform.position, transform.GetChild(0).position));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Only the "nose" of the car should look --> Don't care if only booty is in trigger
        if (other.CompareTag("CarLook"))
        {
            hasCollision = false;
            //Debug.Log($"Exit! {other.gameObject.name}");

        }
    }




}
