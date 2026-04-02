using Gameplay.Entities.Character;
using Unity.Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private CharacterControl _characterControl;
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    
    [Header("Zoom Settings")]
    [SerializeField] private Vector2 _orthographicRange = new(8f, 12f);
    [SerializeField] private float _zoomSpeed = 2f;

    [Header("Forward Focus (Lookahead)")]
    [SerializeField] private float _offsetMultiplier = 0.6f; 
    [SerializeField] private float _maxOffset = 5f;        
    [SerializeField] private float _offsetSmoothing = 2f;    

    private Rigidbody2D _rigidbody;
    private CinemachineFollow _followComponent;

    private float _initialOrthographicSize;
    private Vector3 _initialFollowOffset;

    public void Start()
    {
        _rigidbody = _characterControl.GetComponent<Rigidbody2D>();
        _followComponent = _cinemachineCamera.GetComponent<CinemachineFollow>();
    
        _initialOrthographicSize = _orthographicRange.x;
        _initialFollowOffset = _followComponent.FollowOffset;
    
        _cinemachineCamera.Lens.OrthographicSize = _initialOrthographicSize;
    }

    private void OnDestroy()
    {
        if (_followComponent != null)
            _followComponent.FollowOffset = _initialFollowOffset;
    
        if (_cinemachineCamera != null)
            _cinemachineCamera.Lens.OrthographicSize = _initialOrthographicSize;
    }

    private void Update()
    {
        if (_rigidbody == null || _followComponent == null)
        {
            return;
        }

        float currentSpeed = _rigidbody.linearVelocity.magnitude;
        float maxSpeed = _characterControl.MovementControl.MaxSpeed;
        float speedFactor = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);

        float targetSize = Mathf.Lerp(_orthographicRange.x, _orthographicRange.y, speedFactor);
        _cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(_cinemachineCamera.Lens.OrthographicSize, 
            targetSize, Time.deltaTime * _zoomSpeed);
        
        Vector3 targetOffset = (_rigidbody.linearVelocity * _offsetMultiplier);
        targetOffset = Vector3.ClampMagnitude(targetOffset, _maxOffset);
        targetOffset.z = -10f;
        _followComponent.FollowOffset = Vector3.Lerp(_followComponent.FollowOffset, targetOffset, 
            Time.deltaTime * _offsetSmoothing);
    }
}