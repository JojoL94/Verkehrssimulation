using UnityEngine;

public class KameraSteuerung : MonoBehaviour
{
    [SerializeField] float kameraRotationsgeschwindigkeit = 3.0f;
    [SerializeField] float kameraBewegungsgeschwindigkeit = 3.0f;
    private bool isRotating;
    
    [SerializeField] float kameraZoomgeschwindigkeit = 5.0f;
    [SerializeField] float kameraMinZoom = 10f; // Minimale Feldgröße für den Zoom
    [SerializeField] float kameraMaxZoom = 60f; // Maximale Feldgröße für den Zoom
    [SerializeField] float kameraHöhenverstellgeschwindigkeit = 2.0f;
    [SerializeField] float minKameraHöhe = 10f;
    [SerializeField] float maxKameraHöhe = 20f;

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
        
        // Kamerahöhe mit den Tasten STRG (nach unten) und Shift (nach oben) ändern
        float höhenverstellung = 0f;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            höhenverstellung = -kameraHöhenverstellgeschwindigkeit * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            höhenverstellung = kameraHöhenverstellgeschwindigkeit * Time.deltaTime;
        }

        // Anwenden der Höhenverstellung und sicherstellen, dass die Höhe im zulässigen Bereich bleibt
        transform.Translate(0, höhenverstellung, 0);
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.Clamp(newPosition.y, minKameraHöhe, maxKameraHöhe);
        transform.position = newPosition;
    }
}