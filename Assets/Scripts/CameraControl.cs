using Gameplay.Entities.Character;
using Unity.Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _mainMenuCamera;
    [SerializeField] private CinemachineCamera _gameplayCamera;

    [Header("Zoom Settings")] [SerializeField]
    private Vector2 _orthographicRange = new(8f, 12f);

    [SerializeField] private float _zoomSpeed = 2f;

    [Header("Forward Focus (Lookahead)")] [SerializeField]
    private float _offsetMultiplier = 0.6f;

    [SerializeField] private float _maxOffset = 5f;
    [SerializeField] private float _offsetSmoothing = 2f;

    private CharacterControl _characterControl;
    private Rigidbody2D _rigidbody;
    private CinemachineFollow _followComponent;
    private CinemachineCamera _activeCamera;

    private bool _isGameplayActive;

    public void Initialize(CharacterControl characterControl)
    {
        _characterControl = characterControl;
        _mainMenuCamera.Follow = _characterControl.transform;
        _mainMenuCamera.LookAt = _characterControl.transform;
        _gameplayCamera.Follow = _characterControl.transform;
        _gameplayCamera.LookAt = _characterControl.transform;
        _rigidbody = _characterControl.GetComponent<Rigidbody2D>();
        _followComponent = _gameplayCamera.GetComponent<CinemachineFollow>();

        _gameplayCamera.Lens.OrthographicSize = _orthographicRange.x;
    }

    private void Update()
    {
        if (!_isGameplayActive) return;

        UpdateZoom();
        UpdateFollowOffset();
    }

    public void ActivateMainMenuCamera()
    {
        _isGameplayActive = false;
        SwitchCamera(_mainMenuCamera);
    }

    public void ActivateGameplayCamera()
    {
        _isGameplayActive = true;
        SwitchCamera(_gameplayCamera);
    }

    private void UpdateZoom()
    {
        float speedFactor = Mathf.InverseLerp(0f, _characterControl.MovementControl.MaxSpeed,
            _rigidbody.linearVelocity.magnitude);

        float targetSize = Mathf.Lerp(_orthographicRange.x, _orthographicRange.y, speedFactor);

        _activeCamera.Lens.OrthographicSize = Mathf.Lerp(
            _activeCamera.Lens.OrthographicSize,
            targetSize,
            Time.deltaTime * _zoomSpeed);
    }

    private void UpdateFollowOffset()
    {
        Vector3 targetOffset = Vector3.ClampMagnitude(
            _rigidbody.linearVelocity * _offsetMultiplier,
            _maxOffset);
        targetOffset.z = -10f;

        _followComponent.FollowOffset = Vector3.Lerp(
            _followComponent.FollowOffset,
            targetOffset,
            Time.deltaTime * _offsetSmoothing);
    }

    private void SwitchCamera(CinemachineCamera cameraToActivate)
    {
        if (_activeCamera != null)
            _activeCamera.Priority = 0;

        _activeCamera = cameraToActivate;
        _activeCamera.Priority = 10;
    }
}