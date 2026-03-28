using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Gameplay.Data
{
    [CreateAssetMenu(fileName = "ConfigurationCollection", menuName = "ScriptableObjects/Configuration/Collection")]
    public class ConfigurationCollection : ScriptableObject
    {
        [SerializeField] private AirplaneConfig _airplaneConfig;
        [SerializeField] private SlingshotRopeConfig _slingshotRopeConfig;
        [SerializeField] private CurrencyMultiplierConfig _currencyMultiplierConfig;
        [SerializeField] private RocketConfig _rocketConfig;

        public AirplaneConfig AirplaneConfig => _airplaneConfig;
        public SlingshotRopeConfig SlingshotRopeConfig => _slingshotRopeConfig;
        public CurrencyMultiplierConfig CurrencyMultiplierConfig => _currencyMultiplierConfig;
        public RocketConfig RocketConfig => _rocketConfig;


        public Dictionary<ConfigurationType, BaseConfigurationConfig> Configurations => new()
        {
            { ConfigurationType.Plane, _airplaneConfig },
            { ConfigurationType.Slingshot, _slingshotRopeConfig },
            { ConfigurationType.Income, _currencyMultiplierConfig },
            { ConfigurationType.Rocket, _rocketConfig }
        };

        public int GetUpgradeCost(ConfigurationType type, int level)
        {
            var config = Configurations[type] as ConfigurationConfig;

            return config != null ? config.GetUpgradeCost(level) : 0;
        }
    }
}