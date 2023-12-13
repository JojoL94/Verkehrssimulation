using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarInfoView : MonoBehaviour
{
    // Start is called before the first frame update

    public static CarInfoView instance;

    [SerializeField] private GameObject carInfoView;
    [SerializeField] private TextMeshProUGUI origin, destination, speed,averageSpeed, drivingStyle, carId;
    [SerializeField] private Image carPhoto;
    
    public List<Sprite> carPhotos;

    private void Awake()
    {
        instance = this;
        carInfoView.SetActive(false);
    }

    private void Update()
    {
        if(carInfoView.activeInHierarchy)
            if (_prevCar)
            {
                // 30 = in realLife, 6 = in game
                float kmh = Mathf.RoundToInt(30 / 6 * _prevCar.speed);
                speed.text = kmh+"km/h";
                
                float avgKmh = Mathf.RoundToInt(30 / 6 * _prevCar.averageSpeed);
                averageSpeed.text = avgKmh+"km/h";
            }
    }

    private MoveCar _prevCar;
    public void ChangeCarInfoTo(MoveCar car)
    {
        if (car == _prevCar) // Clicked twice on same car --> disable UI
        {
            Debug.Log("Clicked same car twice");
            _prevCar = null;
            carInfoView.SetActive(false);
        }
        else
        {
            carInfoView.SetActive(true);
            origin.text = car.origin.name;
            destination.text = car.destination.name;
            carId.text = "#"+car.carID;
            carPhoto.sprite = carPhotos[car.prefabId - 1];


            switch (car.aggressivenessLevel)
            {
                case 0:
                    drivingStyle.text = "Fährt wie Opa"; 
                    break;
                case 1: 
                    drivingStyle.text = "Sonntagsfahrer"; 
                    break;
                case 2: 
                    drivingStyle.text = "Normal"; 
                    break;
                case 3: 
                    drivingStyle.text = "Rasant"; 
                    break;
                case 4: 
                    drivingStyle.text = "Bekloppt"; 
                    break;
            }
            
            _prevCar = car;
        }
    }
    
}
