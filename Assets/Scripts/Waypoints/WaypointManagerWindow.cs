using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


//Manager to create new Waypoints
public class WaypointManagerWindow : EditorWindow
{
    //Variable to store the waypointCollection (empty GameObject, acting as both, a folder and a list)
    public Transform waypointCollection;

    //Create new editor and define path to open it
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open() 
    {
        GetWindow<WaypointManagerWindow>();
    }



    //Editor window
    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        //Field in Editor to add the waypointCollection GameObject
        EditorGUILayout.PropertyField(obj.FindProperty("waypointCollection"));

        //Check if waypointCollection was added, if not => print warning
        if (waypointCollection == null)
        {
            EditorGUILayout.HelpBox("Please assign a root Transform", MessageType.Warning);
        }
        //If waypointCollection was added...
        else
        {
            //...add a new dark grey box...
            EditorGUILayout.BeginVertical("box");

            //...add a new button inside this box, which executes CreateWaypoint() if clicked...
            if (GUILayout.Button("Create Waypoint"))
            {
                CreateWaypoint();
            }
            //... stopp the dark grex box
            EditorGUILayout.EndVertical();
        }

        //Always check if waypointCollection was changed and if so, adjust accordingly
        obj.ApplyModifiedProperties();
    }



    //Creates a new Waypoint and link to previous Waypoint
    void CreateWaypoint() 
    {
        //Create new Waypoint
        GameObject waypointObject = new GameObject("Waypoint" + waypointCollection.childCount, typeof(Waypoint));

        //Add Waypoint as Child to waypointCollection
        waypointObject.transform.SetParent(waypointCollection, false);

        //Open Script "Waypoint" of Components
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        //If not the first Waypoint (which has no predecessor)
        if (waypointCollection.childCount > 1) 
        {
            //Add predecessor as previous Waypoint
            waypoint.GetComponent<Waypoint>().neighbours.Add(waypointCollection.GetChild(waypointCollection.childCount - 2).GetComponent<Waypoint>().transform);

            //Add current Waypoint as successor to predecessor
            waypoint.GetComponent<Waypoint>().neighbours[0].GetComponent<Waypoint>().neighbours.Add(waypoint.transform);

            //Place new Waypoint at position of first predecessor
            waypoint.transform.position = waypoint.neighbours[0].transform.position;
            waypoint.transform.forward = waypoint.neighbours[0].transform.forward;
        }

        //Select the new Waypoint
        Selection.activeObject = waypoint.gameObject;
    }
}
