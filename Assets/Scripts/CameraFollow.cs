using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Об'єкт, за яким стежимо
    public float smoothSpeed = 0.125f; // Швидкість згладжування (чим менше, тим повільніше)
    public Vector3 offset = new Vector3(0, 0, -10); // Відступ (Z має бути -10 для 2D камери)

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            // Використовуйте Time.smoothDeltaTime для ще більшої стабільності
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime * 10f);
        }
    }
}