using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Datenvisualisierung : MonoBehaviour
{
    public TextMeshProUGUI carCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        carCounter.SetText("Test");
    }
}
