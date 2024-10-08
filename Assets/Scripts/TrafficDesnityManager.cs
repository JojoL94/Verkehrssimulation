using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficDesnityManager : MonoBehaviour
{
    public static TrafficDesnityManager instance;
    private bool _showingDensity;

    public List<TrafficDensity> trafficDensities = new List<TrafficDensity>();

    public Material whiteMaterial, groundMaterial;

    public Texture whiteTree, normalTree;
    public Material mainTree;

    public Material noTrafficMat, // azure
        lightTrafficMat, // green
        moderateTrafficMat, // yellow
        heavyTrafficMat, // orange
        trafficJamMat, // red
        trafficChaosMat; // darkRed


    public Transform housesParent;
    private List<Material> _housesStandard = new List<Material>();

    public Image colorFeedbackImg;
    public Button colorButton;

    public Transform[] grounds;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < housesParent.childCount; i++)
            _housesStandard.Add(housesParent.GetChild(i).GetComponent<MeshRenderer>().materials[0]);
        /*for (int i = 0; i < treeParent.childCount; i++)
            _treeStandard.Add(treeParent.GetChild(i).GetComponent<MeshRenderer>().materials[0]);*/
        mainTree.SetTexture("_MainTex", normalTree);
    }

    public void SwitchOnOff()
    {
        _showingDensity = !_showingDensity;
        GameManager.instance.SwitchButtonState(_showingDensity, colorButton);
        if (_showingDensity)
        {
            // Change Houses
            for (int i = 0; i < housesParent.childCount; i++)
            {
                Material[] materials = housesParent.GetChild(i).GetComponent<MeshRenderer>().materials;
                materials[0] = whiteMaterial;
                housesParent.GetChild(i).GetComponent<MeshRenderer>().materials = materials;
            }
            
            // Change Tree
            mainTree.SetTexture("_MainTex", whiteTree);

            foreach(Transform ground in grounds)
            {
                // Change Ground
                Material[] materialsGround = ground.GetComponent<MeshRenderer>().materials;
                materialsGround[0] = whiteMaterial;
                ground.GetComponent<MeshRenderer>().materials = materialsGround;
            }
            
            
        }
        else
        {
            // Change Houses
            for (int i = 0; i < housesParent.childCount; i++)
            {
                Material[] materials = housesParent.GetChild(i).GetComponent<MeshRenderer>().materials;
                materials[0] = _housesStandard[i];
                housesParent.GetChild(i).GetComponent<MeshRenderer>().materials = materials;
            }
            
            // Change Tree
            mainTree.SetTexture("_MainTex", normalTree);

            foreach (Transform ground in grounds)
            {
                // Change Ground
                Material[] materialsGround = ground.GetComponent<MeshRenderer>().materials;
                materialsGround[0] = groundMaterial;
                ground.GetComponent<MeshRenderer>().materials = materialsGround;
            }
        }

        foreach (TrafficDensity trafficDensity in trafficDensities)
        {
            if (trafficDensity.changingColors != null)
            {
                StopCoroutine(trafficDensity.changingColors);
                trafficDensity.changingColors = null;
            }

            // Change Roads
            if (_showingDensity)
                trafficDensity.changingColors = StartCoroutine(trafficDensity.ChangeColors());
            else trafficDensity.SetStandardMaterial();
        }
    }
}