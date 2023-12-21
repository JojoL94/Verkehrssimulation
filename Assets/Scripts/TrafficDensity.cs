using System;
using System.Collections;
using System.Collections.Generic;
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

    private float averageSpeed;
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

        _trafficDesnityManager.trafficDensities.Add(this);
    }

    private RaycastHit[] hits;
    void OnDrawGizmos()
    {
        Vector3 raycastDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.right;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.TransformPoint(center), transform.TransformDirection(raycastDirection) * raycastLength);

        RaycastHit[] hits = Physics.RaycastAll(transform.TransformPoint(center),
            transform.TransformDirection(raycastDirection), raycastLength, layerMask);

        foreach (RaycastHit hit in hits)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hit.point, 0.2f);
        }
    }

    void Update()
    {
        Vector3 raycastDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.right;

        Debug.DrawRay(transform.TransformPoint(center), transform.TransformDirection(raycastDirection) * raycastLength,
            color);

        hits = Physics.RaycastAll(transform.TransformPoint(center),
            transform.TransformDirection(raycastDirection), raycastLength, layerMask);

        foreach (RaycastHit hit in hits)
        {
            averageSpeed = hit.collider.gameObject.transform.parent.GetComponent<MoveCar>().speed;
            maxSpeed = hit.collider.gameObject.transform.parent.GetComponent<MoveCar>().maxSpeed;
        }
    }

    public IEnumerator ChangeColors()
    {
        while (true)
        {
            // IMPORTANT! Colors can't go from green immediately to red!! Only one step at a time
            yield return new WaitForSeconds(4f);
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

            Material[] materials = _renderer.materials;
            materials[0] = _activeMaterial;
            _renderer.materials = materials;
        }
    }

    public void SetStandardMaterial()
    {
        _renderer.material = _standardMaterial;
    }
}