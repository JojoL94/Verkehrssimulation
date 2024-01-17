using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrafficDensity : MonoBehaviour
{
    public LayerMask layerMask; // Layer-Mask, um zu definieren, auf welche Layer der Raycast reagieren soll
    public float raycastLength; // Die Länge des Raycasts
    private RaycastHit hit;
    private Color color = new Color(191, 192, 57);
    public Vector3 center;

    private Material _standardMaterial;

    private Material _noTrafficMat, // azure
        _lightTrafficMat, // green
        _moderateTrafficMat, // yellow
        _heavyTrafficMat, // orange
        _trafficJamMat, // red
        _trafficChaosMat; // darkRed

    private Material _activeMaterial;

    private TrafficDesnityManager _trafficDesnityManager;

    private MeshRenderer _renderer;

    public Coroutine changingColors;

    public float averageSpeed;
    private float maxSpeed;
    public float rotationAngle = 0f;
    
    private void Start()
    {
        _trafficDesnityManager = TrafficDesnityManager.instance;
        _noTrafficMat = _trafficDesnityManager.noTrafficMat;
        _lightTrafficMat = _trafficDesnityManager.lightTrafficMat;
        _moderateTrafficMat = _trafficDesnityManager.moderateTrafficMat;
        _heavyTrafficMat = _trafficDesnityManager.heavyTrafficMat;
        _trafficJamMat = _trafficDesnityManager.trafficJamMat;
        _trafficChaosMat = _trafficDesnityManager.trafficChaosMat;

        /*_noTrafficLimit = _trafficDesnityManager.noTrafficLimit;
        _lightTrafficLimit = _trafficDesnityManager.lightTrafficLimit;
        _moderateTrafficLimit = _trafficDesnityManager.moderateTrafficLimit;
        _heavyTrafficLimit = _trafficDesnityManager.heavyTrafficLimit;
        _trafficJamLimit = _trafficDesnityManager.trafficJamLimit;*/

        _renderer = GetComponent<MeshRenderer>();
        _standardMaterial = _renderer.materials[0];
        _trafficDesnityManager.trafficDensities.Add(this);

        averageSpeed = -1;

        StartCoroutine(UpdateOneSecond());
    }

    //private RaycastHit[] hits;
    public List<GameObject> hits = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car")) 
        {
            //Check if car is already inside hist to prevent duplicates
            if (!hits.Contains(other.gameObject)) {
                other.gameObject.GetComponent<MoveCar>().trafficDensity = this.GetComponent<TrafficDensity>();
                hits.Add(other.gameObject);
            }

            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            if (other.gameObject != null) 
            {
                hits.Remove(other.gameObject);
            }             
        }
    }

        void OnDrawGizmos()
    {
        /**Vector3 raycastDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.right;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.TransformPoint(center), transform.TransformDirection(raycastDirection) * raycastLength);

        RaycastHit[] hits = Physics.RaycastAll(transform.TransformPoint(center),
            transform.TransformDirection(raycastDirection), raycastLength, layerMask);

        foreach (RaycastHit hit in hits)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hit.point, 0.2f);
        }*/
    }

    void Update()
    {
        /**Vector3 raycastDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.right;

        Debug.DrawRay(transform.TransformPoint(center), transform.TransformDirection(raycastDirection) * raycastLength,
            color);

        hits = Physics.RaycastAll(transform.TransformPoint(center),
            transform.TransformDirection(raycastDirection), raycastLength, layerMask);
        */

        /**averageSpeed = -1;
        /**foreach (RaycastHit hit in hits) 
        {
            averageSpeed = hit.collider.gameObject.transform.parent.GetComponent<MoveCar>().speed;
            maxSpeed = hit.collider.gameObject.transform.parent.GetComponent<MoveCar>().maxSpeed;
        }*/
    }

    IEnumerator UpdateOneSecond()
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));
        while (true)
        {
            averageSpeed = -1;
            if (hits.Count > 0)
            {
                for (int y = 0; y < hits.Count; y++)
                {
                    if (hits[y] == null)
                    {
                        hits.Remove(hits[y]);
                    }
                    else
                    {
                        averageSpeed = hits[y].GetComponent<MoveCar>().speed;
                        maxSpeed = hits[y].GetComponent<MoveCar>().maxSpeed;
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
    

    public IEnumerator ChangeColors()
    {
        while (true)
        {
            // IMPORTANT! Colors can't go from green immediately to red!! Only one step at a time
            
            if (averageSpeed/maxSpeed > .95f)
            {
                _activeMaterial = _lightTrafficMat;
            }
            else if (averageSpeed/maxSpeed > .80f)
            {
                _activeMaterial = _lightTrafficMat;
            }
            else if (averageSpeed/maxSpeed > .50f)
            {
                _activeMaterial = _moderateTrafficMat;
            }
            else if (averageSpeed/maxSpeed > .25f)
            {
                _activeMaterial = _heavyTrafficMat;
            }
            else if (averageSpeed/maxSpeed > .10f)
            {
                _activeMaterial = _trafficJamMat;
            }
            else
            {
                _activeMaterial = _trafficChaosMat;
            }
            if(averageSpeed==-1)
                _activeMaterial = _lightTrafficMat;
            
            Material[] materials = _renderer.materials;
            materials[0] = _activeMaterial;
            _renderer.materials = materials;
            yield return new WaitForSeconds(4f);
        }
    }

    public void SetStandardMaterial()
    {
        _renderer.material = _standardMaterial;
    }
}