using System.Collections.Generic;
using Gameplay.Data;
using Services.Currency;
using Services.Storage;

namespace UI.Screens.Variants.MainMenu
{
    public class ConfigurationDataService
    {
        private StorageService _storageService;
        private CurrencyService _currencyService;

        public ConfigurationCollection ConfigurationCollection { get; private set; }
        public List<ConfigurationData> ConfigurationData { get; private set; } = new();


        public void Init(ConfigurationCollection collection)
        {
            ConfigurationCollection = collection;
            _storageService = ServicesManager.Instance.StorageService;
            _currencyService = ServicesManager.Instance.CurrencyService;
            InitializeConfigurations();
            ConfigurationData = _storageService.LoadData(StorageConstants.CONFIGURATION, ConfigurationData);
        }

        private void InitializeConfigurations()
        {
            foreach (var config in ConfigurationCollection.Configurations)
            {
                var data = new ConfigurationData
                {
                    Type = config.Key,
                    IsUnlocked = config.Key != ConfigurationType.Rocket,
                    Level = config.Key == ConfigurationType.Rocket ? 0 : 1,
                    AdditionalLevel = 1,
                };
                ConfigurationData.Add(data);
            }
        }

        public void ResetLevelData()
        {
            ConfigurationData.Find(data => data.Type == ConfigurationType.Body).AdditionalLevel = 1;
            ConfigurationData.Find(data => data.Type == ConfigurationType.Rocket).Level = 0;
            ConfigurationData.Find(data => data.Type == ConfigurationType.Rocket).IsUnlocked = false;

            _storageService.SaveData(StorageConstants.CONFIGURATION, ConfigurationData);
        }

        public bool TryUpgrade(ConfigurationType type, out int nextLevel, out int upgradeCost)
        {
            var data = ConfigurationData.Find(data => data.Type == type);
            nextLevel = data.Level + 1;
            var nextAdditionalLevel = data.AdditionalLevel + 1;
            upgradeCost = ConfigurationCollection.GetUpgradeCost(type, data.Level);

            var currency = _currencyService.GetCurrencyByType(CurrencyType.Coin);
            if (currency.Currency >= upgradeCost)
            {
                currency.SpendCurrency(upgradeCost);
                data.Level = nextLevel;
                data.AdditionalLevel = nextAdditionalLevel;
                upgradeCost = ConfigurationCollection.GetUpgradeCost(type, nextLevel);
                _storageService.SaveData(StorageConstants.CONFIGURATION, ConfigurationData);
                return true;
            }

            return false;
        }

        public void UpgradeRocket()
        {
            var rocketData = ConfigurationData.Find(data => data.Type == ConfigurationType.Rocket);
            rocketData.Level++;
            rocketData.IsUnlocked = true;
            _storageService.SaveData(StorageConstants.CONFIGURATION, ConfigurationData);
        }

        public ConfigurationData GetConfigurationData(ConfigurationType type)
            => ConfigurationData.Find(data => data.Type == type);
    }
}