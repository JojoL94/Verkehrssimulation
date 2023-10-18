using UnityEngine;

public class KameraSteuerung : MonoBehaviour
{
    [SerializeField] float kameraRotationsgeschwindigkeit = 3.0f;
    [SerializeField] float kameraBewegungsgeschwindigkeit = 3.0f;
    private bool isRotating;
    
    [SerializeField] float kameraZoomgeschwindigkeit = 5.0f;
    [SerializeField] float kameraMinZoom = 10f; // Minimale Feldgröße für den Zoom
    [SerializeField] float kameraMaxZoom = 60f; // Maximale Feldgröße für den Zoom


    void Update()
    {
        // Kamerarotation mit rechter Maustaste und Mausbewegung
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            // Kamerarotation mit Mausbewegung
            float horizontalRotation = Input.GetAxis("Mouse X") * kameraRotationsgeschwindigkeit;
            float verticalRotation = Input.GetAxis("Mouse Y") * kameraRotationsgeschwindigkeit * -1;
            transform.Rotate(0, horizontalRotation, 0);
            transform.GetChild(0).Rotate(verticalRotation, 0, 0);
        }
        
        // Kamerabewegung mit den Tasten W, A, S, D
        float horizontalBewegung = Input.GetAxis("Horizontal") * kameraBewegungsgeschwindigkeit * Time.deltaTime;
        float vertikaleBewegung = Input.GetAxis("Vertical") * kameraBewegungsgeschwindigkeit * Time.deltaTime;
        transform.Translate(new Vector3(horizontalBewegung, 0, vertikaleBewegung));

        // Kamerazoom mit dem Mausrad ändern
        float zoom = Input.GetAxis("Mouse ScrollWheel") * kameraZoomgeschwindigkeit;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoom, kameraMinZoom, kameraMaxZoom);
    }
}