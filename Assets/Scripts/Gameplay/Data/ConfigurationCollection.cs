using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Gameplay.Data
{
    [CreateAssetMenu(fileName = "ConfigurationCollection", menuName = "ScriptableObjects/Configuration/Collection")]
    public class ConfigurationCollection : ScriptableObject
    {
        [SerializeField] private BodyConfig _bodyConfig;
        [SerializeField] private MainEngineConfig _mainEngineConfig;
        [SerializeField] private CurrencyMultiplierConfig _currencyMultiplierConfig;
        [SerializeField] private AdditionalEngineConfig _additionalEngineConfig;

        public BodyConfig BodyConfig => _bodyConfig;
        public MainEngineConfig MainEngineConfig => _mainEngineConfig;
        public CurrencyMultiplierConfig CurrencyMultiplierConfig => _currencyMultiplierConfig;
        public AdditionalEngineConfig AdditionalEngineConfig => _additionalEngineConfig;


        public Dictionary<ConfigurationType, BaseConfigurationConfig> Configurations => new()
        {
            { ConfigurationType.Body, _bodyConfig },
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