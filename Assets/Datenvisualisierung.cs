using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Datenvisualisierung : MonoBehaviour
{
    public TextMeshProUGUI carCounterTextMesh;

    public TextMeshProUGUI carsAverageSpeedTextMesh;

    public TextMeshProUGUI placeHolderTextMesh;
    
    private int carCounter;

    private int oldCarCounter = 0;
    private float oldAverageSpeed = 0;

    private int tmpCarID;
    public List<MoveCar> cars = new List<MoveCar>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        carCounter = cars.Count;
        if (carCounter != oldCarCounter)
        {
            carCounterTextMesh.SetText(carCounter.ToString());
            oldCarCounter = carCounter;
        }

        float tmpAverageSpeed = 0;
        
        foreach (var moveCar in cars)
        {
            tmpAverageSpeed += moveCar.speed;
        }

        tmpAverageSpeed /= cars.Count;
        if (tmpAverageSpeed >= oldAverageSpeed + 0.3f || tmpAverageSpeed <= oldAverageSpeed - 0.3f)
        {
            oldAverageSpeed = Mathf.Lerp(oldAverageSpeed, tmpAverageSpeed, 0.5f);
            oldAverageSpeed = Mathf.Round(oldAverageSpeed * 100f) / 100f;
            carsAverageSpeedTextMesh.SetText(oldAverageSpeed.ToString());
        }

    }

    public int AddCarInDatenVisualisierung(MoveCar tmpMoveCar)
    {
        tmpCarID++;
        cars.Add(tmpMoveCar);
        return tmpCarID;
    }

    public void RemoveCarInDatenVisualisierung(MoveCar tmpMoveCar)
    {
        cars.Remove(tmpMoveCar);
    }
}
