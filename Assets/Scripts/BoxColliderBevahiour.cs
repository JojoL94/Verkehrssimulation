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
