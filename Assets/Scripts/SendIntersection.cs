using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SendIntersection : MonoBehaviour
{

    // Die Länge des Raycasts
    float raycastDistance = 7.5f;

    // Führe den eigentlichen Raycast durch
    RaycastHit hitSend;

    public bool hasCollision = false;
    public GameObject hittingCar;

    public LayerMask layerMask;
    

    private void Start()
    {
        layerMask = LayerMask.GetMask("Car");
    }

    private void Update()
    {
        // Checks if someone is standing in the intersection
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * raycastDistance, Color.yellow); // Zeichne den Raycast in der Szene
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hitSend, raycastDistance, layerMask))
        {                                             
            // Überprüfen, ob das getroffene Objekt ein Auto ist
            hasCollision = true;
            hittingCar = hitSend.transform.gameObject;
            if (hittingCar.GetComponent<MoveCar>() == null)
                hittingCar = hittingCar.transform.parent.gameObject;
        }
        else
        {
            hasCollision = false;
            hittingCar = null;
        }
    }
}
