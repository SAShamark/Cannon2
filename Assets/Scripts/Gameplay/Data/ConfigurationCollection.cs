using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Gameplay.Data
{
    [CreateAssetMenu(fileName = "ConfigurationCollection", menuName = "ScriptableObjects/Configuration/Collection")]
    public class ConfigurationCollection : ScriptableObject
    {
        [SerializeField] private RocketBodyConfig _rocketBodyConfig;
        [SerializeField] private MainEngineConfig _mainEngineConfig;
        [SerializeField] private CurrencyMultiplierConfig _currencyMultiplierConfig;
        [SerializeField] private AdditionalEngineConfig _additionalEngineConfig;

        public RocketBodyConfig RocketBodyConfig => _rocketBodyConfig;
        public MainEngineConfig MainEngineConfig => _mainEngineConfig;
        public CurrencyMultiplierConfig CurrencyMultiplierConfig => _currencyMultiplierConfig;
        public AdditionalEngineConfig AdditionalEngineConfig => _additionalEngineConfig;


        public Dictionary<ConfigurationType, BaseConfigurationConfig> Configurations => new()
        {
            { ConfigurationType.Body, _rocketBodyConfig },
            { ConfigurationType.Engine, _mainEngineConfig },
            { ConfigurationType.Income, _currencyMultiplierConfig },
            { ConfigurationType.Rocket, _additionalEngineConfig }
        };

        public int GetUpgradeCost(ConfigurationType type, int level)
        {
            var config = Configurations[type] as ConfigurationConfig;

            return config != null ? config.GetUpgradeCost(level) : 0;
        }
    }
}