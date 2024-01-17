using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SendIntersection : MonoBehaviour
{

    // Die L�nge des Raycasts
    public float raycastDistance = 4.5f;

    // F�hre den eigentlichen Raycast durch
    RaycastHit hitSend;

    public bool hasCollision = false;
    public GameObject hittingCar;

    private LayerMask layerMask;

    public float offset = 1f;
    

    private void Start()
    {
        layerMask = LayerMask.GetMask("Car");
        offset = 1f;
    }

    private void Update()
    {
        Vector3 localOffset = transform.parent.InverseTransformDirection(Vector3.right *2f*offset); // Lokaler Offset um 2 Einheiten auf der y-Achse im Parent-Objekt
        // Checks if someone is standing in the intersection
        Debug.DrawRay(GetRaycastStartPosition(), transform.TransformDirection(Vector3.back*offset) * raycastDistance, Color.red); // Zeichne den Raycast in der Szene
        if (Physics.Raycast(GetRaycastStartPosition(), transform.TransformDirection(Vector3.back*offset), out hitSend, raycastDistance, layerMask))
        {                                             
            // �berpr�fen, ob das getroffene Objekt ein Auto ist
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

    void OnDrawGizmos()
    {
        // Zeichne den Raycast mit dem Offset im Editor
        Gizmos.color = Color.red;
        Gizmos.DrawRay(GetRaycastStartPosition(), GetRaycastDirection() * -raycastDistance);
    }
    Vector3 GetRaycastStartPosition()
    {
        // Berechne den Startpunkt des Raycasts mit dem Offset
        return transform.position + transform.TransformDirection(Vector3.back * 3 * offset);
    }

    Vector3 GetRaycastDirection()
    {
        // Gib die Richtung des Raycasts basierend auf der aktuellen Rotation zur�ck
        return transform.forward;
    }

}

