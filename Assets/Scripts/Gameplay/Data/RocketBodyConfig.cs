using UnityEngine;

namespace Gameplay.Data
{
    [CreateAssetMenu(fileName = "RocketBody", menuName = "ScriptableObjects/Configuration/RocketBody")]
    public class RocketBodyConfig : ConfigurationConfig
    {
        [Header("Fuel")]
        [SerializeField] private float _baseFuel = 100f;
        [SerializeField] private float _fuelPerUpgrade = 15f;

        [Header("Thrust")]
        [SerializeField] private float _baseThrustMultiplier = 1f;
        [SerializeField] private float _thrustPerUpgrade = 0.1f;

        public float GetMaxFuel(int upgradeLevel)
        {
            return _baseFuel + upgradeLevel * _fuelPerUpgrade;
        }

        public float GetThrustMultiplier(int upgradeLevel)
        {
            return _baseThrustMultiplier + upgradeLevel * _thrustPerUpgrade;
        }
    }
}