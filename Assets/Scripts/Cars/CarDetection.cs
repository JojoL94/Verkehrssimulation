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
    public float minTargetDistance = 1.5f;
    private float targetDistanceModifier = 100f;
    private float raycastOffset = 2.7f;

    //Variablen für das Bremsen
    private float myBrakeModifier;

    //Variablen für das Überholen
    private bool overtake;
    void Start()
    {
        myMoveCar = GetComponent<MoveCar>();
        myBrakeModifier = myMoveCar.brakeDecelerationModifier;
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
                    //Store Skript as variable for performance reasons
                    MoveCar moveCarCollider = hit.collider.GetComponent<MoveCar>();
                    if (
                        hit.collider.GetComponent<CarDetection>().carInFront != transform)
                    {
                        if (hit.collider.GetComponent<Transform>() != transform)
                        {
                            if (Vector3.Distance(hit.point, objectPosition) < targetDistanceToFrontCar)
                            {
                                myBrakeModifier = 100 - ((Vector3.Distance(hit.point, objectPosition) / targetDistanceToFrontCar) * 100);
                                myMoveCar.brakeDecelerationModifier = myBrakeModifier;
                                tmpDoBrake = true;
                            }
                            else
                            {
                                myBrakeModifier = 0;
                            }

                            if (moveCarCollider.nextLocalWaypoint == myMoveCar.nextLocalWaypoint ||
                                moveCarCollider.nextLocalWaypoint == myMoveCar.lastLocalWaypoint ||
                                moveCarCollider.lastLocalWaypoint == myMoveCar.nextLocalWaypoint ||
                                moveCarCollider.lastLocalWaypoint == myMoveCar.lastLocalWaypoint)
                            {
                                // Wenn das getroffene Objekt den richtigen Tag hat und in die gleiche Richtung fährt, speichere es als carInFront
                                carInFront = hit.collider.gameObject.transform;
                                carInFrontDetected = true;
                            }
                        }
                    }
                }

                //Wenn lane switch trigger
                if (hit.collider.CompareTag("Trigger"))
                {
                    if (hit.collider.gameObject.transform.GetSiblingIndex() == 0)
                    {
                        //Wenn anderes Auto Spur wechseln möchte
                        if (hit.collider.gameObject.transform.parent.GetChild(1).GetComponent<checkLaneTrigger>().checkLaneQueue == true)
                        {
                            //The closer the car to other lane, the stronger the stopping => prevent car from entering occupied lane
                            float normalizedDistance = Mathf.Clamp01(Vector3.Distance(this.transform.position, myMoveCar.nextLocalWaypoint.transform.position) - 0.5f / 10f);
                            float test = 17f * (1 - normalizedDistance);
                            myMoveCar.doBrake = true;

                            myMoveCar.speed -= test * Time.deltaTime;
                            myMoveCar.speed = Mathf.Max(myMoveCar.speed, 0);
                        }
                    }

                    if (hit.collider.gameObject.transform.GetSiblingIndex() == 1)
                    {
                        //Wenn anderes Auto Spur wechseln möchte
                        if (hit.collider.gameObject.transform.parent.GetChild(0).GetComponent<checkLaneTrigger>().checkLaneQueue == true)
                        {
                            //The closer the car to other lane, the stronger the stopping => prevent car from entering occupied lane
                            float normalizedDistance = Mathf.Clamp01(Vector3.Distance(this.transform.position, myMoveCar.nextLocalWaypoint.transform.position) - 0.5f / 10f);
                            float test = 17f * (1 - normalizedDistance);
                            myMoveCar.doBrake = true;

                            myMoveCar.speed -= test * Time.deltaTime;
                            myMoveCar.speed = Mathf.Max(myMoveCar.speed, 0);
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

                //Überhole Auto wenn mehrere Bedinungen erfüllt sind:
                //- Geschwindigkeit des vorherigen Autos ist langsamer als aktuelles Auto
                //- bool overtake ist false (overtake ist trigger um mehrfaches Ausführen von switchLane() zu verhindern)
                //- Es gibt einen nexBigWaypoint (wird für die folgende Bedingung gebraucht)
                //- Abstand zum nexBigWaypoint ist größer als 20 => Kein Überholen nach dem Trigger zum Einordnen an den Kreuzungen, um Bugs zu unterbinden
                //- Befindet sich auf einer 4 spurigen Straße (prüft "ShadowWaypoint", welches 4 spurige Straße signalisiert)               
                if (carInFront.GetComponent<MoveCar>().speed < myMoveCar.speed
                    && overtake == false
                    && myMoveCar.nexBigWaypoint != null
                    && Vector3.Distance(this.transform.position, myMoveCar.nexBigWaypoint.transform.position) > 20
                    && myMoveCar.nextLocalWaypoint.transform.parent.name.Contains("ShadowWaypoint"))
                    { 
                    //Überprüfe ob nebenliegende Fahrbahn frei ist, überhole nur, wenn das der Fall ist
                    if (myMoveCar.nextLocalWaypoint.transform.GetSiblingIndex() == 0)
                    {
                        //- Streckenabschnitt des nächsten Waypoints auf der anderen Fahrbahn ist leer (jener Waypoint, den das Auto durch Überholen erreichen möchte)
                        //- Streckenabschnitt des vorherigen Waypoints auf der anderen Fahrbahn ist leer (damit das Autos niemanden beim Überholen schneidet)
                        if (myMoveCar.nextLocalWaypoint.transform.parent.GetChild(1).gameObject.GetComponent<ShadowWaypoint>().carsOnLane == 0
                            && myMoveCar.lastLocalWaypoint.transform.parent.GetChild(1).gameObject.GetComponent<ShadowWaypoint>().carsOnLane == 0) 
                        {
                            myMoveCar.switchLane();
                            overtake = true;
                            Debug.Log(this.name + "overtake");
                        }
                    }
                    else
                    {
                        //- Streckenabschnitt des nächsten Waypoints auf der anderen Fahrbahn ist leer (jener Waypoint, den das Auto durch Überholen erreichen möchte)
                        //- Streckenabschnitt des vorherigen Waypoints auf der anderen Fahrbahn ist leer (damit das Autos niemanden beim Überholen schneidet)
                        if (myMoveCar.nextLocalWaypoint.transform.parent.GetChild(0).gameObject.GetComponent<ShadowWaypoint>().carsOnLane == 0
                            && myMoveCar.lastLocalWaypoint.transform.parent.GetChild(0).gameObject.GetComponent<ShadowWaypoint>().carsOnLane == 0)
                        {
                            myMoveCar.switchLane();
                            overtake = true;
                            Debug.Log(this.name + "overtake");
                        }
                    }      
                }
                
                //Halte Abstand zum gefundenem Auto
                if (Vector3.Distance(objectPosition, carInFront.position) < targetDistanceToFrontCar)
                {
                    myBrakeModifier = 100 - ((Vector3.Distance(objectPosition, carInFront.position) / targetDistanceToFrontCar) * 100);
                    myMoveCar.brakeDecelerationModifier = myBrakeModifier;
                    tmpDoBrake = true;
                }
                else
                {
                    myBrakeModifier = 0;
                    carInFrontDetected = false;
                    overtake = false;
                }
            }
            else
            {
                myBrakeModifier = 0;
                carInFrontDetected = false;
                overtake = false;
            }

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                if (hit.collider.CompareTag("Car"))
                {
                    //Store Skript as variable for performance reasons
                    MoveCar moveCarCollider = hit.collider.GetComponent<MoveCar>();
                    if (
                        hit.collider.GetComponent<CarDetection>().carInFront != transform)
                    {
                        if (hit.collider.GetComponent<Transform>() != transform)
                        {
                            if (Vector3.Distance(hit.point, objectPosition) < targetDistanceToFrontCar)
                            {
                                myBrakeModifier = 100 - ((Vector3.Distance(hit.point, objectPosition) / targetDistanceToFrontCar) * 100);
                                myMoveCar.brakeDecelerationModifier = myBrakeModifier;
                                tmpDoBrake = true;
                            }
                            else
                            {
                                myBrakeModifier = 0;
                            }

                            if (moveCarCollider.nextLocalWaypoint == myMoveCar.nextLocalWaypoint ||
                                moveCarCollider.nextLocalWaypoint == myMoveCar.lastLocalWaypoint ||
                                moveCarCollider.lastLocalWaypoint == myMoveCar.nextLocalWaypoint ||
                                moveCarCollider.lastLocalWaypoint == myMoveCar.lastLocalWaypoint)
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