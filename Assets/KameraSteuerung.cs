using UnityEngine;

public class KameraSteuerung : MonoBehaviour
{
    public float rotationsgeschwindigkeit = 3.0f;
    public float bewegungsgeschwindigkeit = 3.0f;
    public float scrollgeschwindigkeit = 3.0f;

    void Update()
    {
        // Kamerarotation mit Mausbewegung
        float horizontalRotation = Input.GetAxis("Mouse X") * rotationsgeschwindigkeit;
        transform.Rotate(0, horizontalRotation, 0);

        // Kamerabewegung mit den Tasten W, A, S, D
        float horizontalBewegung = Input.GetAxis("Horizontal") * bewegungsgeschwindigkeit * Time.deltaTime;
        float vertikaleBewegung = Input.GetAxis("Vertical") * bewegungsgeschwindigkeit * Time.deltaTime;
        transform.Translate(new Vector3(horizontalBewegung, 0, vertikaleBewegung));

        // Kamerahöhe mit dem Mausrad ändern
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollgeschwindigkeit;
        transform.Translate(0, scroll, 0);
    }
}