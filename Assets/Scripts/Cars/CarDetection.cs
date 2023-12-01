using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private float minTargetDistance = 1.5f;
    private float targetDistanceModifier = 100f;
    private float raycastOffset = 2.7f;

    void Start()
    {
        myMoveCar = GetComponent<MoveCar>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 objectPosition = transform.position + transform.right * raycastOffset;
        bool tmpDoBrake = false;
        // Berechnen Sie den gewünschten Abstand basierend auf der Geschwindigkeit
        targetDistanceToFrontCar =
            minTargetDistance + myMoveCar.speed;
        Vector3 raycastDirection = transform.right;
        // Erstelle einen Raycast von der aktuellen Position in Richtung der Vorwärtsansicht (z.B., Z-Achse)
        Ray ray = new Ray(objectPosition, raycastDirection);
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
                        if (hit.collider.GetComponent<Transform>() != transform)
                        {
                            if (Vector3.Distance(hit.point, objectPosition) < targetDistanceToFrontCar)
                            {
                                tmpDoBrake = true;
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
            }

            Debug.DrawRay(objectPosition + Vector3.back, raycastDirection * raycastDistance,
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
                if (Vector3.Distance(objectPosition, carInFront.position) < targetDistanceToFrontCar)
                {
                    tmpDoBrake = true;
                }
                else
                {
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
                        if (hit.collider.GetComponent<Transform>() != transform)
                        {
                            if (Vector3.Distance(hit.point, objectPosition) < targetDistanceToFrontCar)
                            {
                                tmpDoBrake = true;
                            }

                            if (hit.collider.GetComponent<MoveCar>().nextLocalWaypoint == myMoveCar.nextLocalWaypoint ||
                                hit.collider.GetComponent<MoveCar>().nextLocalWaypoint == myMoveCar.lastLocalWaypoint ||
                                hit.collider.GetComponent<MoveCar>().lastLocalWaypoint == myMoveCar.nextLocalWaypoint ||
                                hit.collider.GetComponent<MoveCar>().lastLocalWaypoint == myMoveCar.lastLocalWaypoint)
                            {
                                // Wenn das getroffene Objekt den richtigen Tag hat und in die gleiche Richtung fährt, speichere es als carInFront
                                carInFront = hit.collider.gameObject.transform;
                            }
                        }
                    }
                }
            }
        }

        if (Physics.Raycast(transform.position, raycastDirection, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Car"))
            {
                if (hit.collider.GetComponent<Transform>() != transform)
                {
                    if (
                        hit.collider.GetComponent<CarDetection>().carInFront != transform)
                    {
                        carInFront = hit.collider.gameObject.transform;
                        tmpDoBrake = true;
                    }
                }
            }

            Debug.DrawRay(transform.position - transform.right / 2,
                raycastDirection * Vector3.Distance(transform.position, hit.point),
                Color.yellow); // Zeichne den Raycast in der Szene
        }

        if (Vector3.Distance(transform.position, myMoveCar.nextLocalWaypoint.transform.position) <
            targetDistanceToFrontCar)
        {
            raycastDirection = myMoveCar.nextLocalWaypoint.transform.position - transform.position;
            if (Physics.Raycast(objectPosition, raycastDirection, out hit, raycastDistance))
            {
                if (hit.collider.CompareTag("Car"))
                {
                    if (hit.collider.GetComponent<MoveCar>().lastLocalWaypoint == myMoveCar.nextLocalWaypoint)
                    {
                        if (hit.collider.GetComponent<Transform>() != transform)
                        {
                            // Wenn das getroffene Objekt den richtigen Tag hat und in die gleiche Richtung fährt, trigger brake
                            tmpDoBrake = true;
                        }
                    }
                }

                Debug.DrawRay(transform.position - transform.right / 4,
                    raycastDirection * Vector3.Distance(transform.position, hit.point),
                    Color.blue); // Zeichne den Raycast in der Szene
            }
        }

        myMoveCar.doBrake = tmpDoBrake;
    }

    public void SwitchLane()
    {
        carInFront = null;
        carInFrontDetected = false;
    }
}