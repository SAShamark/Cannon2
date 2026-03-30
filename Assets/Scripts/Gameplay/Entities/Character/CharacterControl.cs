using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterControl : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private List<ParticleSystem> _mainRocketsVFX;
        [SerializeField] private MovementControl _movementControl;
        [SerializeField] private AdditionalRockets _additionalRockets;

        private bool _isInitialized;
        private FloatingJoystick _joystick;

        public FuelControl FuelControl { get; private set; } = new();
        public AdditionalRockets AdditionalRockets => _additionalRockets;
        public MovementControl MovementControl => _movementControl;

        public void Initialize(FloatingJoystick joystick, float fuelValue)
        {
            _joystick = joystick;
            FuelControl.Init(fuelValue);
            _additionalRockets.FuelControl.Init(fuelValue);
            _mainRocketsVFX[0].transform.parent.gameObject.SetActive(true);
            _isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
            {
                return;
            }

            var input = _joystick.Direction;
            var isJoystickPressed = _joystick.IsPointerDown;
            var hasFuel = FuelControl.HasFuel;

            _movementControl.ApplyMovement(_rigidbody2D, input, hasFuel && isJoystickPressed);

            if (hasFuel && isJoystickPressed)
            {
                var mainIntensity = 1f;
                if (input.y < 0)
                {
                    mainIntensity = 1f + input.y;
                }

                FuelControl.ConsumeFuel(Time.fixedDeltaTime * mainIntensity);

                UpdateVFXIntensity(_mainRocketsVFX, mainIntensity);

                var sideIntensity = input.y > 0.1f ? input.y : 0f;
                if (sideIntensity > 0f)
                {
                    _additionalRockets.FuelControl.ConsumeFuel(Time.fixedDeltaTime * sideIntensity);
                }

                UpdateVFXIntensity(_additionalRockets.AdditionRocketsVFX, sideIntensity);
            }
            else
            {
                UpdateVFXIntensity(_mainRocketsVFX, 0f);
                UpdateVFXIntensity(_additionalRockets.AdditionRocketsVFX, 0f);
            }
        }

        private void UpdateVFXIntensity(List<ParticleSystem> particles, float intensity)
        {
            foreach (var ps in particles)
            {
                var emission = ps.emission;
                emission.enabled = intensity > 0.01f;

                if (emission.enabled)
                {
                    emission.rateOverTimeMultiplier = intensity * 50f;
                    var main = ps.main;
                    main.startSizeMultiplier = Mathf.Lerp(0.4f, 1.1f, intensity);
                }
            }
        }
    }
}