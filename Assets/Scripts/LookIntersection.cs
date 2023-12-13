using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookIntersection : MonoBehaviour
{
    // Die Richtung des Raycasts (in diesem Fall in die Vorw�rtsrichtung des GameObjects)
    Vector3 raycastDirection;

    // Die L�nge des Raycasts
    public float raycastDistance = 4f;

    // F�hre den eigentlichen Raycast durch
    RaycastHit hitSend;

    public bool hasCollision = false;
    public GameObject hittingCar;

    public LayerMask layerMask;

    public enum Behaviour { LookForOtherCar, SendCar };
    public Behaviour behaviour;

    private void Start()
    {
        // Die Richtung des Raycasts (in diesem Fall in die Vorw�rtsrichtung des GameObjects)
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
            {                                                                                         // �berpr�fen, ob das getroffene Objekt ein Auto ist
                hasCollision = true;
                hittingCar = hitSend.transform.gameObject;
            }
            else hasCollision = false;
        }
    }
}
