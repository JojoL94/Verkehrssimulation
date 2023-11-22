using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookIntersection : MonoBehaviour
{
    // Die Richtung des Raycasts (in diesem Fall in die Vorwärtsrichtung des GameObjects)
    Vector3 raycastDirection;

    // Die Länge des Raycasts
    public float raycastDistance = 5f;

    // Führe den eigentlichen Raycast durch
    RaycastHit hitSend;

    public bool hasCollision = false;
    public GameObject hittingCar;

    public LayerMask layerMask;

    public enum Behaviour { LookForOtherCar, SendCar };
    public Behaviour behaviour;

    private void Start()
    {
        // Die Richtung des Raycasts (in diesem Fall in die Vorwärtsrichtung des GameObjects)
        raycastDirection = -transform.forward;
        layerMask = LayerMask.GetMask("Car");
        StartCoroutine(checkRaycast());
    }

    IEnumerator checkRaycast()
    {
        while (true)
        {
            yield return new WaitForSeconds(.01f);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * raycastDistance, Color.red); // Zeichne den Raycast in der Szene
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hitSend, raycastDistance, layerMask))
            {                                                                                         // Überprüfen, ob das getroffene Objekt ein Auto ist
                hasCollision = true;
                hittingCar = hitSend.transform.gameObject;
            }
            else hasCollision = false;
        }
    }
}
