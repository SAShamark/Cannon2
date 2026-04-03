using System;
using System.Collections.Generic;
using Gameplay.Entities.Background;
using Gameplay.Entities.Items;
using Services.Currency;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterControl : MonoBehaviour, ICatchHandler
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private MovementControl _movementControl;
        [SerializeField] private FuelControl _fuelControl;
        [SerializeField] private List<ParticleSystem> _mainRocketsVFX;
        [SerializeField] private AdditionalRockets _additionalRockets;
        [SerializeField] private GameObject _magnet;
        [SerializeField] private HealthControl _healthControl;
        public bool IsLaunched { get; private set; }
        private FloatingJoystick _joystick;

        public HealthControl HealthControl => _healthControl;
        public FuelControl FuelControl => _fuelControl;
        public AdditionalRockets AdditionalRockets => _additionalRockets;
        public MovementControl MovementControl => _movementControl;

        public event Action OnLevelFinished;

        public void Initialize(FloatingJoystick joystick, float fuelPercent)
        {
            _joystick = joystick;
            FuelControl.Init(fuelPercent);
            _healthControl.Init();
            _mainRocketsVFX[0].transform.parent.gameObject.SetActive(true);
            _additionalRockets.Init(1);
            IsLaunched = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsLaunched) return;
            ContactPoint2D contact = collision.GetContact(0);
            _healthControl.TakeCollisionDamage(collision.relativeVelocity, contact.normal);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            collision.TryGetComponent(out FinishTileControl finishTile);
            if (finishTile != null)
            {
                OnLevelFinished?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            if (!IsLaunched)
            {
                return;
            }

            var input = _joystick.Direction;
            var isJoystickPressed = _joystick.IsPointerDown;
            var hasFuel = FuelControl.HasFuel;
            var hasAdditionalFuel = _additionalRockets.FuelControl.HasFuel;

            _movementControl.ApplyMovement(_rigidbody2D, input, hasFuel && isJoystickPressed, hasAdditionalFuel);

            if (hasFuel && isJoystickPressed)
            {
                var mainIntensity = 1f;
                if (input.y < 0)
                {
                    mainIntensity = 1f + input.y;
                }

                FuelControl.ConsumeFuel(Time.fixedDeltaTime * mainIntensity);
                UpdateVFXIntensity(_mainRocketsVFX, mainIntensity);

                var sideIntensity = hasAdditionalFuel && input.y > 0.1f ? input.y : 0f;

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

        public void EarnCurrency(CurrencyType type, int value)
        {
            //ServicesManager.Instance.CurrencyService.GetCurrencyByType(type).EarnCurrency(value);
        }

        public void EarnFuel(int value)
        {
            FuelControl.RefillFuel(value);
        }

        public void GetMagnet(float radius, float duration)
        {
            _magnet.SetActive(true);
        }

        public void GetAccelerator(float percent, float duration)
        {
            //_movementControl
        }
    }
}