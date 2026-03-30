using System;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    public class FuelControl
    {
        private float _maxFuel;
        private float _currentFuel;
        private float _consumptionRate = 1f;

        public float CurrentFuel => _currentFuel;
        private float FuelRatio => _maxFuel > 0 ? _currentFuel / _maxFuel : 0f;
        public bool HasFuel => _currentFuel > 0f;

        public event Action<float> OnFuelChanged;

        public void Init(float fuelValue, float consumptionRate = 1f)
        {
            _maxFuel = fuelValue;
            _currentFuel = fuelValue;
            _consumptionRate = consumptionRate;
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