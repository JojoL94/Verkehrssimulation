using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Datenvisualisierung : MonoBehaviour
{
    public TextMeshProUGUI carCounterTextMesh;

    public TextMeshProUGUI carsSpeedMeanTextMesh;

    public TextMeshProUGUI placeHolderTextMesh;
    
    public int carCounter;

    private int oldCarCounter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (carCounter != oldCarCounter)
        {
            carCounterTextMesh.SetText("Cars on the Street: " + carCounter.ToString());
            oldCarCounter = carCounter;
        }
    }
}
