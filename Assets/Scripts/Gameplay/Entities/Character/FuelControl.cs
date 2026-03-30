using System;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    [Serializable]
    public class FuelControl
    {
        [SerializeField] private float _consumptionRate = 1f;
        [SerializeField] private float _maxFuel = 100;

        private float _currentFuel;

        public float CurrentFuel => _currentFuel;
        private float FuelRatio => _maxFuel > 0 ? _currentFuel / _maxFuel : 0f;
        public bool HasFuel => _currentFuel > 0f;

        public event Action<float> OnFuelChanged;

        public void Init(float fuelPercent, float maxFuel = 100f)
        {
            _currentFuel = fuelPercent * _maxFuel;
        }

        public void ConsumeFuel(float deltaTime)
        {
            _currentFuel = Mathf.Max(0f, _currentFuel - _consumptionRate * deltaTime);
            OnFuelChanged?.Invoke(FuelRatio);
        }

        public void RefillFuel(float amount)
        {
            _currentFuel = Mathf.Min(_maxFuel, _currentFuel + amount);
            OnFuelChanged?.Invoke(FuelRatio);
        }
    }
}