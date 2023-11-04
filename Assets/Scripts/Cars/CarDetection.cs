using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetection : MonoBehaviour
{
    //Variablen um andere Autos zu erkennen
    private MoveCar myMoveCar;
    private bool carInFrontDetected;
    public Transform carInFront;
    private float raycastDistance = 20;

    //Variablen für den Abstand
    public float targetDistanceToFrontCar;
    private float minTargetDistance = 5.5f;
    private float targetDistanceModifier = 10f;

    void Start()
    {
        myMoveCar = GetComponent<MoveCar>();
    }

    // Update is called once per frame
    void Update()
    {
        // Berechnen Sie den gewünschten Abstand basierend auf der Geschwindigkeit
        targetDistanceToFrontCar =
            minTargetDistance + (myMoveCar.speed / myMoveCar.maxSpeed) * targetDistanceModifier;
        Vector3 raycastDirection = transform.right;
        // Erstelle einen Raycast von der aktuellen Position in Richtung der Vorwärtsansicht (z.B., Z-Achse)
        Ray ray = new Ray(transform.position, raycastDirection);
        RaycastHit hit;

        // Überprüfe, ob der Raycast ein Objekt mit dem Tag "Car" trifft
        if (!carInFrontDetected)
        {
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                if (hit.collider.CompareTag("Car"))
                {
                    if (
                        hit.collider.GetComponent<CarDetection>().carInFront != transform)
                    {
                        if (Vector3.Distance(hit.point, transform.position) < targetDistanceToFrontCar)
                        {
                            myMoveCar.doBrake = true;
                        }

                        if (hit.collider.GetComponent<MoveCar>().nextLocalWaypoint == myMoveCar.nextLocalWaypoint ||
                            hit.collider.GetComponent<MoveCar>().nextLocalWaypoint == myMoveCar.lastLocalWaypoint ||
                            hit.collider.GetComponent<MoveCar>().lastLocalWaypoint == myMoveCar.nextLocalWaypoint ||
                            hit.collider.GetComponent<MoveCar>().lastLocalWaypoint == myMoveCar.lastLocalWaypoint)
                        {
                            // Wenn das getroffene Objekt den richtigen Tag hat und in die gleiche Richtung fährt, speichere es als carInFront
                            carInFront = hit.collider.gameObject.transform;
                            carInFrontDetected = true;
                        }
                    }
                }
            }

            Debug.DrawRay(transform.position + Vector3.back, raycastDirection * raycastDistance,
                Color.magenta); // Zeichne den Raycast in der Szene
        }
        else
        {
            if (carInFront != null)
            {
                Debug.DrawRay(transform.position + Vector3.forward,
                    (carInFront.position - transform.position) * targetDistanceToFrontCar,
                    Color.cyan); // Zeichne den Raycast in der Szene
                //Halte Abstand zum gefundenem Auto
                if (Vector3.Distance(transform.position, carInFront.position) < targetDistanceToFrontCar)
                {
                    myMoveCar.doBrake = true;
                }
                else
                {
                    myMoveCar.doBrake = false;
                    carInFrontDetected = false;
                }
            }
            else
            {
                carInFrontDetected = false;
            }

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                if (hit.collider.CompareTag("Car"))
                {
                    if (
                        hit.collider.GetComponent<CarDetection>().carInFront != transform)
                    {
                        if (Vector3.Distance(hit.point, transform.position) < targetDistanceToFrontCar)
                        {
                            myMoveCar.doBrake = true;
                        }
                    }
                }
            }
        }
    }

    public void SwitchLane()
    {
        carInFront = null;
        carInFrontDetected = false;
    }
}