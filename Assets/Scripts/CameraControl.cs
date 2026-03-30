using Gameplay.Entities.Character;
using Unity.Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private CharacterControl _characterControl;
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Vector2 _orthographicRange = new(8f, 12f);
    [SerializeField] private float _zoomSpeed = 2f; // Швидкість зміни зуму

    private Rigidbody2D _rb;

    public void Start()
    {
        // Отримуємо Rigidbody через CharacterControl
        _rb = _characterControl.GetComponent<Rigidbody2D>();
        _cinemachineCamera.Lens.OrthographicSize = _orthographicRange.x;
    }

    private void Update()
    {
        if (_rb == null) return;

        // 1. Отримуємо поточну швидкість (векторну довжину)
        float currentSpeed = _rb.linearVelocity.magnitude;
        
        // 2. Визначаємо "прогрес" швидкості від 0 до максимуму (від 0 до 1)
        float maxSpeed = _characterControl.MovementControl.MaxSpeed;
        float speedFactor = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);

        // 3. Розраховуємо цільовий розмір камери
        float targetSize = Mathf.Lerp(_orthographicRange.x, _orthographicRange.y, speedFactor);

        // 4. Плавно змінюємо поточний розмір на цільовий
        _cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(
            _cinemachineCamera.Lens.OrthographicSize, 
            targetSize, 
            Time.deltaTime * _zoomSpeed
        );
    }
}