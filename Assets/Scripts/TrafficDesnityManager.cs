using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficDesnityManager : MonoBehaviour
{
    public static TrafficDesnityManager instance;
    private bool _showingDensity;

    public List<TrafficDensity> trafficDensities = new List<TrafficDensity>();

    public Material whiteMaterial, groundMaterial;
    public Material noTrafficMat, // azure
        lightTrafficMat, // green
        moderateTrafficMat, // yellow
        heavyTrafficMat, // orange
        trafficJamMat, // red
        trafficChaosMat; // darkRed

    public int noTrafficLimit, // azure
        lightTrafficLimit, // green
        moderateTrafficLimit, // yellow
        heavyTrafficLimit, // orange
        trafficJamLimit; // red
    

    public Transform housesParent;
    private List<Material> _housesStandard = new List<Material>();
    
    public Transform treeParent;
    private List<Material> _treeStandard = new List<Material>();

    public Transform ground;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < housesParent.childCount; i++)
            _housesStandard.Add(housesParent.GetChild(i).GetComponent<MeshRenderer>().materials[0]);
        for (int i = 0; i < treeParent.childCount; i++)
            _treeStandard.Add(treeParent.GetChild(i).GetComponent<MeshRenderer>().materials[0]);
        
    }

    public void SwitchOnOff()
    {
        _showingDensity = !_showingDensity;

        foreach (TrafficDensity trafficDensity in trafficDensities)
        {
            if (trafficDensity.changingColors != null)
            {
                StopCoroutine(trafficDensity.changingColors);
                trafficDensity.changingColors = null;
            }

            if (_showingDensity)
            {
                // Change Roads
                trafficDensity.changingColors = StartCoroutine(trafficDensity.ChangeColors());
                
                // Change Houses
                for (int i = 0; i < housesParent.childCount; i++)
                {
                    Material[] materials = housesParent.GetChild(i).GetComponent<MeshRenderer>().materials;
                    materials[0] = whiteMaterial;
                    housesParent.GetChild(i).GetComponent<MeshRenderer>().materials = materials;
                }
                
                // Change Trees
                for (int i = 0; i < treeParent.childCount; i++)
                {
                    Material[] materials = treeParent.GetChild(i).GetComponent<MeshRenderer>().materials;
                    materials[0] = whiteMaterial;
                    treeParent.GetChild(i).GetComponent<MeshRenderer>().materials = materials;
                }
                // Change Ground
                
                Material[] materialsGround = ground.GetComponent<MeshRenderer>().materials;
                materialsGround[0] = whiteMaterial;
                ground.GetComponent<MeshRenderer>().materials = materialsGround;
            }
            else
            {
                // Change Roads
                trafficDensity.SetStandardMaterial();
                
                // Change Houses
                for (int i = 0; i < housesParent.childCount; i++)
                {
                    Material[] materials = housesParent.GetChild(i).GetComponent<MeshRenderer>().materials;
                    materials[0] = _housesStandard[i];
                    housesParent.GetChild(i).GetComponent<MeshRenderer>().materials = materials;
                }

                // Change Trees
                for (int i = 0; i < treeParent.childCount; i++)
                {
                    Material[] materials = treeParent.GetChild(i).GetComponent<MeshRenderer>().materials;
                    materials[0] = _treeStandard[i];
                    treeParent.GetChild(i).GetComponent<MeshRenderer>().materials = materials;
                }
                // Change Ground
                Material[] materialsGround = ground.GetComponent<MeshRenderer>().materials;
                materialsGround[0] = groundMaterial;
                ground.GetComponent<MeshRenderer>().materials = materialsGround;
            }
        }
    }
}