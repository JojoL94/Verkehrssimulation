using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetection : MonoBehaviour
{
    //Variablen um andere Autos zu erkennen
    private MoveCar myMoveCar;
    private bool carInFrontDetected;
    public Transform carInFront;
    private float raycastDistance = 10;

    //Variablen für den Abstand
    public float targetDistanceToFrontCar;
    private float minTargetDistance = 2f;
    private float targetDistanceModifier = 5f;

    void Start()
    {
        myMoveCar = GetComponent<MoveCar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!carInFrontDetected)
        {
            Vector3 raycastDirection = transform.right;
            // Erstelle einen Raycast von der aktuellen Position in Richtung der Vorwärtsansicht (z.B., Z-Achse)
            Ray ray = new Ray(transform.position, raycastDirection);
            RaycastHit hit;

            // Überprüfe, ob der Raycast ein Objekt mit dem Tag "Car" trifft
            if (Physics.Raycast(ray, out hit, raycastDistance) && hit.collider.CompareTag("Car") &&
                hit.collider.GetComponent<MoveCar>().nexBigWaypoint == myMoveCar.nexBigWaypoint && hit.collider.GetComponent<CarDetection>().carInFront != transform)
            {
                // Wenn das getroffene Objekt den richtigen Tag hat und in die gleiche Richtung fährt, speichere es als carInFront
                carInFront = hit.collider.gameObject.transform;
                carInFrontDetected = true;
            }

            Debug.DrawRay(transform.position, raycastDirection * raycastDistance,
                Color.magenta); // Zeichne den Raycast in der Szene
        }
        else
        {
            if (carInFront != null)
            {
                // Berechnen Sie den gewünschten Abstand basierend auf der Geschwindigkeit
                targetDistanceToFrontCar =
                    minTargetDistance + (myMoveCar.speed / myMoveCar.maxSpeed) * targetDistanceModifier;
                Debug.DrawRay(transform.position, (carInFront.position - transform.position) * targetDistanceToFrontCar,
                    Color.cyan); // Zeichne den Raycast in der Szene
                //Halte Abstand zum gefundenem Auto
                if (Vector3.Distance(transform.position, carInFront.position) < targetDistanceToFrontCar)
                {
                    myMoveCar.doBrake = true;
                }
                else
                {
                    myMoveCar.doBrake = false;
                }
            }
            else
            {
                carInFrontDetected = false;
            }
        }
    }

    public void SwitchLane()
    {
        carInFront = null;
        carInFrontDetected = false;
    }
}