using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


//Manager to create new Waypoints
public class StreetManagerWindow : EditorWindow
{
   /** //Variable to store PreFab of the four lane highway street
    public GameObject fourLaneHighwayStreet;

    //Variable to store the waypointCollection (empty GameObject, acting as both, a folder and a list)
    public Transform waypointCollection;

    //Variable to store the streetCollection (empty GameObject, acting as both, a folder and a list)
    public Transform streetCollection;

    //Create new editor and define path to open it
    [MenuItem("Tools/Street Editor")]
    public static void Open() 
    {
        GetWindow<StreetManagerWindow>();
    }



    //Editor window
    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        //Field in Editor to add the waypointCollection GameObject
        EditorGUILayout.PropertyField(obj.FindProperty("waypointCollection"));

        //Field in Editor to add the streetCollection GameObject
        EditorGUILayout.PropertyField(obj.FindProperty("streetCollection"));

        //Check if waypointCollection was added, if not => print warning
        if (waypointCollection == null)
        {
            EditorGUILayout.HelpBox("Please assign the collection for waypoints", MessageType.Warning);

            //Check if streetCollection was added, if not => print warning
        }
        else if(streetCollection == null) 
        
        {
            //Check if streetCollection was added, if not => print warning
            EditorGUILayout.HelpBox("Please assign the collection for streets", MessageType.Warning);
        }
        //If waypointCollection and streetCollection were added...
        else
        {
            //...add a new dark grey box...
            EditorGUILayout.BeginVertical("box");

            //...add a new button inside this box, which executes CreateWaypoint() if clicked...
            if (GUILayout.Button("Create 4-lane street"))
            {
                Create4LaneStreet();
            }
            //... stopp the dark grex box
            EditorGUILayout.EndVertical();
        }

        //Always check if waypointCollection was changed and if so, adjust accordingly
        obj.ApplyModifiedProperties();
    }

    GameObject CreateStreetFromPreFab(GameObject PreFab) {

        //Create new streetObject
        GameObject streetObject = Instantiate(PreFab);

        //Add streetObject at positon of last added streetObject
        streetObject.transform.position = streetCollection.transform.GetChild(streetCollection.transform.childCount - 1).position;
        streetObject.transform.rotation = streetCollection.transform.GetChild(streetCollection.transform.childCount - 1).rotation;

        //Add streetObject as child to streetCollection
        streetObject.transform.SetParent(streetCollection);

        //Select the new street
        Selection.activeObject = streetObject.gameObject;

        return streetObject;
    }

     GameObject CreateMainWaypoint(GameObject streetForWaypoint) {

        //Create new Waypoint
        GameObject waypointObject = new GameObject("Waypoint" + waypointCollection.childCount, typeof(Waypoint));

        //Add Waypoint as Child to waypointCollection
        waypointObject.transform.SetParent(waypointCollection, false);


        waypointObject.transform.rotation = streetForWaypoint.transform.rotation;

        //Set position of waypointObject
        waypointObject.transform.position =
        new UnityEngine.Vector3(
            streetForWaypoint.transform.position.x, 
            streetForWaypoint.transform.position.y, 
            streetForWaypoint.transform.position.z);


        //Open Script "Waypoint" of Components
        waypointObject.GetComponent<Waypoint>();


        return waypointObject;
    }


    //Creates a new Waypoint and link to previous Waypoint
    void Create4LaneStreet() 
    {
        /**
         * -------------------------------------------------------------------
         * Street 
         * -------------------------------------------------------------------
         */

        //Create new streetObject
        //GameObject streetObject = CreateStreetFromPreFab(fourLaneHighwayStreet);



        /**
         * -------------------------------------------------------------------
         * Waypoints
         * -------------------------------------------------------------------
         */

        //Create new Waypoint
        //GameObject waypointObject = CreateMainWaypoint(streetObject);
    //}
}
