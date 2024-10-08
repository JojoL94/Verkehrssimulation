using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

//GameManager is responsible for everything that's effecting the entire map (like max number of cars on map, etc.)
public class GameManager : MonoBehaviour
{
    //Max number of cars driving at the same time
    public float maxCars = 30;
    public float currentCars = 0;

    public TMP_InputField newMaxCars;

    public int carUID = 0;

    public Transform waypointCollection;
    public Transform streetCollection;

    //Max distance of Raycast
    public float RaycastDistance = 100f;

    //Counter to give every Main Waypoint an uniqe name
    private int counter = 0;

    public GameObject feierabend, normal;

    //Variables for "Feierabend" Feedback
    public Button feierabendButton;

    public Color yellow, black;

    public static GameManager instance;

    // Displays maxCars
    public TextMeshProUGUI maxCarsText;
    
    //In runtime connect all Waypoints before start
    private void Awake()
    {
        carUID = 0;
        instance = this;
        //Iterate through streetCollection
        for (int x = 0; x < streetCollection.childCount; x++)
        {
            //Filter all streets with children
            if (streetCollection.GetChild(x).childCount > 0) {

                //Iterate through all children
                for (int y = 0; y < streetCollection.GetChild(x).transform.childCount; y++) {

                    //Filter out all "RightOfWay" children
                    if (streetCollection.GetChild(x).GetChild(y).name.Contains("Waypoint")) {

                        //Declare RaycastHit
                        RaycastHit hit;

                        //Special case: ShadoWaypoints (Standalone LokalWaypoints)
                        if (streetCollection.GetChild(x).GetChild(y).name.Contains("ShadowWaypoint"))
                        {
                            //For shadowWaypoints
                            //Declare shadowWaypoint
                            Transform shadowWaypoint = streetCollection.GetChild(x).GetChild(y);

                            //Iterate through children
                            for (int z = 0; z < shadowWaypoint.transform.childCount; z++)
                            {
                                //For all other children (lokalWaypoints)
                                //Declare lokalWaypoint
                                GameObject localWaypoint = shadowWaypoint.GetChild(z).gameObject;

                                //Connect all Lokal Waypoints
                                if (Physics.Raycast(localWaypoint.transform.position, localWaypoint.transform.TransformDirection(Vector3.right), out hit, RaycastDistance, LayerMask.GetMask("LokalWaypoint")))
                                {
                                    localWaypoint.GetComponent<LocalWaypoint>().connectedWaypoints.Add(hit.collider.gameObject);
                                }
                            }
                        }
                        else 
                        {
                            //For all other children (mainWaypoints)
                            //Declare mainWaypoint
                            Transform mainWaypoint = streetCollection.GetChild(x).GetChild(y);

                            //Connect all Main Waypoints
                            if (Physics.Raycast(mainWaypoint.transform.position, mainWaypoint.transform.TransformDirection(Vector3.right), out hit, RaycastDistance *2, LayerMask.GetMask("MainWaypoint"))
                            && mainWaypoint.transform.parent != hit.transform.parent)
                            {
                                mainWaypoint.GetComponent<Waypoint>().neighbours.Add(hit.collider.transform);
                            }

                            //Iterate through children
                            for (int z = 0; z < mainWaypoint.transform.childCount; z++)
                            {
                                //For all other children (lokalWaypoints)
                                //Declare lokalWaypoint
                                GameObject localWaypoint = mainWaypoint.GetChild(z).gameObject;

                                //Connect all Lokal Waypoints
                                if (Physics.Raycast(localWaypoint.transform.position, localWaypoint.transform.TransformDirection(Vector3.right), out hit, RaycastDistance, LayerMask.GetMask("LokalWaypoint"))
                                && localWaypoint.transform.parent.parent != hit.transform.parent.parent)
                                {
                                    localWaypoint.GetComponent<LocalWaypoint>().connectedWaypoints.Add(hit.collider.gameObject);
                                }
                            }
                        }
                    }
                 }    

             }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //In runtime put all Waypoints in waypoint list für A* Algorithm
        for (int x = 0; x < streetCollection.childCount; x++) {

                //Filter out Right before Left Collider
                while (streetCollection.GetChild(x).transform.childCount > 0 
                && streetCollection.GetChild(x).GetChild(transform.childCount).name.Contains("Main"))
                {
                    //Name each Waypoint with uniqe name and put in  waypointCollection(A* Algorithm needs Waypoints with uniqe names and in separate List)
                    streetCollection.GetChild(x).GetChild(transform.childCount).name = "Waypoint" + counter;
                    streetCollection.GetChild(x).GetChild(transform.childCount).parent = waypointCollection.transform;
                    counter++;
                }
        }
        normal.SetActive(true);
        feierabend.SetActive(false);
        SwitchButtonState(false, feierabendButton);
    }

    public void ChangeFeierabendVerkehr()
    {
        feierabend.SetActive(!feierabend.activeInHierarchy);

        if(feierabend.activeInHierarchy)
            foreach (Transform child in feierabend.transform)
            {
                if (child.GetComponent<SpawnCar>() != null)
                    child.GetComponent<SpawnCar>().startSpawnCar();
            }
        
        SwitchButtonState(feierabend.activeInHierarchy, feierabendButton);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetTime(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTime(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetTime(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetTime(4);
        }

        maxCarsText.text = maxCars.ToString();
    }

    void SetTime(int index)
    {
        Time.timeScale = index;
    }

    public void SwitchButtonState(bool state, Button button)
    {
        Image stroke= button.transform.Find("Background").transform.Find("Stroke").GetComponent<Image>();
        Image outerStroke = button.transform.Find("Background").transform.Find("OuterStroke").GetComponent<Image>();
        Image fill= button.transform.transform.Find("Background").Find("Fill").GetComponent<Image>();
        
        if (state)
        {
            stroke.color = black;
            outerStroke.color = yellow;
            fill.color = yellow;
        }
        else
        {
            stroke.color = yellow;
            outerStroke.color = black;
            fill.color = black;
        }
    }

    public void ChangeMaxCars()
    {
        maxCars = int.Parse(newMaxCars.text);
    }
}
