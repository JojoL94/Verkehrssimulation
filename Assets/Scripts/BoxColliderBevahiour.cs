using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderBevahiour : MonoBehaviour
{

    public enum Behaviour { LookForOtherCar, SendCar};
    public Behaviour behaviour;

    BoxCollider boxCollider;
    public BoxColliderBevahiour lookAt;

    public bool hasCollision;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other?.gameObject.tag == "CarLook")
        {
            //Debug.Log($"Enter! {other.gameObject.name}");
            hasCollision = true;
            if (behaviour == Behaviour.LookForOtherCar)
            {
                //Debug.Log($"Give wait! {other.gameObject.name}");
                if (lookAt.hasCollision)
                {
                    other.transform.parent.GetComponent<MoveCar>().giveWait(Vector3.Distance(other.transform.position, transform.GetChild(0).position));
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other?.gameObject.tag == "CarLook")
        {
            //Debug.Log($"Stay! {other.gameObject.name}");
            hasCollision = true;
            if (behaviour == Behaviour.LookForOtherCar)
            {
                //Debug.Log($"Give wait! {other.gameObject.name}");
                if (lookAt.hasCollision)
                {
                    other.transform.parent.GetComponent<MoveCar>().giveWait(Vector3.Distance(other.transform.position, transform.GetChild(0).position));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other?.gameObject.tag == "CarLook")
        {
            hasCollision = false;
            //Debug.Log($"Exit! {other.gameObject.name}");

        }
    }




}
