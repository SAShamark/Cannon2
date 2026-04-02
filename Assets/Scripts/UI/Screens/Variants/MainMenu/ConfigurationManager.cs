using System;
using Gameplay.Data;
using Services.Currency;
using Services.Storage;
using UI.Screens.MainMenu;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class ConfigurationManager : IDisposable
    {
        [SerializeField] private ConfigurationCard _airplaneCard;
        [SerializeField] private ConfigurationCard _slingshotRopeCard;
        [SerializeField] private ConfigurationCard _currencyMultiplierCard;
        [SerializeField] private AdsConfigurationCard _adsConfigurationCard;

        private ConfigurationDataService _dataService;
        private CurrencyService _currencyService;
        private StorageService _storageService;
        public AdsConfigurationCard AdsConfigurationCard => _adsConfigurationCard;

        public event Action<ConfigurationType, bool> OnUpgrade;

        public void Initialize(ConfigurationDataService dataService)
        {
            _dataService = dataService;
            _currencyService = ServicesManager.Instance.CurrencyService;
            _storageService = ServicesManager.Instance.StorageService;

            var currentLevel = _storageService.LoadData(StorageConstants.LEVEL_PROGRESS_KEY, 1);
            foreach (var data in _dataService.ConfigurationData)
            {
                int upgradeCost = _dataService.ConfigurationCollection.GetUpgradeCost(data.Type, data.Level);
                switch (data.Type)
                {
                    case ConfigurationType.Body:
                        _airplaneCard.Initialize(
                            _dataService.ConfigurationCollection.RocketBodyConfig.Sprite,
                            _dataService.ConfigurationCollection.RocketBodyConfig.UpgradeCountToNextPart - 1,
                            data.Level, upgradeCost);
                        _airplaneCard.ChangeInteractability(
                            _currencyService.GetCurrencyByType(CurrencyType.Coin).Currency >= upgradeCost);
                        _airplaneCard.OnUpgrade += UpgradeAirplane;

                        break;

                    case ConfigurationType.Engine:
                        _slingshotRopeCard.Initialize(
                            _dataService.ConfigurationCollection.MainEngineConfig.Sprite,
                            _dataService.ConfigurationCollection.MainEngineConfig.UpgradeCountToNextPart - 1,
                            data.Level, upgradeCost);
                        _slingshotRopeCard.ChangeInteractability(
                            _currencyService.GetCurrencyByType(CurrencyType.Coin).Currency >= upgradeCost);
                        _slingshotRopeCard.OnUpgrade += UpgradeSlingshotRope;
                        break;

                    case ConfigurationType.Income:
                        _currencyMultiplierCard.Initialize(
                            _dataService.ConfigurationCollection.CurrencyMultiplierConfig.Sprite,
                            _dataService.ConfigurationCollection.CurrencyMultiplierConfig.UpgradeCountToNextPart - 1,
                            data.Level, upgradeCost,
                            _dataService.ConfigurationCollection.CurrencyMultiplierConfig.GetUpgradeMultiplier(
                                data.Level, currentLevel));
                        _currencyMultiplierCard.ChangeInteractability(
                            _currencyService.GetCurrencyByType(CurrencyType.Coin).Currency >= upgradeCost);
                        _currencyMultiplierCard.OnUpgrade += UpgradeCurrencyMultiplier;
                        break;
                    case ConfigurationType.Rocket:
                        _adsConfigurationCard.Initialize(
                            _dataService.ConfigurationCollection.AdditionalEngineConfig.Sprite, data.Level);
                        _adsConfigurationCard.OnConfigUpgrade += UpgradeRocket;
                        break;
                    default:
                        Debug.LogWarning($"Unknown configuration type: {data.Type}");
                        break;
                }
            }
        }

        private void UpgradeAirplane()
        {
            if (_dataService.TryUpgrade(ConfigurationType.Body, out int newLevel, out int newCost))
            {
                Sprite sprite = _dataService.ConfigurationCollection.RocketBodyConfig.Sprite;
                _airplaneCard.Draw(sprite, newLevel, newCost);
            }

            ChangeCardsInteractability();
            int interval = _dataService.ConfigurationCollection.RocketBodyConfig.UpgradeCountToNextPart;
            bool isCycle = (newLevel - interval) % (interval - 1) == 0;
            OnUpgrade?.Invoke(ConfigurationType.Body, isCycle);
        }

        private void UpgradeSlingshotRope()
        {
            if (_dataService.TryUpgrade(ConfigurationType.Engine, out int newLevel, out int newCost))
            {
                Sprite sprite = _dataService.ConfigurationCollection.MainEngineConfig.Sprite;
                _slingshotRopeCard.Draw(sprite, newLevel, newCost);
            }

            ChangeCardsInteractability();
            int interval = _dataService.ConfigurationCollection.MainEngineConfig.UpgradeCountToNextPart;
            bool isCycle = (newLevel - interval) % (interval - 1) == 0;
            OnUpgrade?.Invoke(ConfigurationType.Engine, isCycle);
        }

        private void UpgradeCurrencyMultiplier()
        {
            if (_dataService.TryUpgrade(ConfigurationType.Income, out int newLevel, out int newCost))
            {
                CurrencyMultiplierConfig config = _dataService.ConfigurationCollection.CurrencyMultiplierConfig;
                var sprites = config.Sprites;
                int cycleIndex = (newLevel - 1) / config.UpgradeCountToNextPart;

                Sprite sprite = cycleIndex >= 0 && cycleIndex < sprites.Count
                    ? sprites[cycleIndex]
                    : config.Sprite;

                if (sprite == config.Sprite)
                {
                    Debug.LogWarning(
                        $"Missing sprite for cycle index {cycleIndex} (level {newLevel}) in CurrencyMultiplierConfig. Using default.");
                }

                var currentLevel = _storageService.LoadData(StorageConstants.LEVEL_PROGRESS_KEY, 1);
                _currencyMultiplierCard.Draw(sprite, newLevel, newCost,
                    config.GetUpgradeMultiplier(newLevel, currentLevel));
                ChangeCardsInteractability();
                int interval = _dataService.ConfigurationCollection.CurrencyMultiplierConfig.UpgradeCountToNextPart;
                bool isCycle = (newLevel - interval) % (interval - 1) == 0;
                OnUpgrade?.Invoke(ConfigurationType.Income, isCycle);
            }

            _currencyMultiplierCard.ChangeInteractability(
                _currencyService.GetCurrencyByType(CurrencyType.Coin).Currency >= newCost);
        }

        private void UpgradeRocket(ConfigurationType type, int level)
        {
            _dataService.UpgradeRocket();
            var sprite = _dataService.ConfigurationCollection.AdditionalEngineConfig.Sprite;
            _adsConfigurationCard.Draw(sprite, level + 1);
            OnUpgrade?.Invoke(ConfigurationType.Rocket, false);
        }

        public void UpdateUI()
        {
            foreach (var data in _dataService.ConfigurationData)
            {
                int upgradeCost = _dataService.ConfigurationCollection.GetUpgradeCost(data.Type, data.Level);
                switch (data.Type)
                {
                    case ConfigurationType.Body:
                        _airplaneCard.Draw(
                            _dataService.ConfigurationCollection.RocketBodyConfig.Sprite, data.Level, upgradeCost);
                        break;
                    case ConfigurationType.Engine:
                        _slingshotRopeCard.Draw(
                            _dataService.ConfigurationCollection.MainEngineConfig.Sprite, data.Level, upgradeCost);
                        break;
                    case ConfigurationType.Income:
                        var currentLevel = _storageService.LoadData(StorageConstants.LEVEL_PROGRESS_KEY, 1);
                        _currencyMultiplierCard.Draw(
                            _dataService.ConfigurationCollection.CurrencyMultiplierConfig.Sprite,
                            data.Level, upgradeCost,
                            _dataService.ConfigurationCollection.CurrencyMultiplierConfig.GetUpgradeMultiplier(
                                data.Level, currentLevel));
                        break;
                    case ConfigurationType.Rocket:
                        _adsConfigurationCard.Draw(_dataService.ConfigurationCollection.AdditionalEngineConfig.Sprite,
                            data.Level);
                        break;
                }
            }

            ChangeCardsInteractability();
        }

        private void ChangeCardsInteractability()
        {
            foreach (var data in _dataService.ConfigurationData)
            {
                int upgradeCost = _dataService.ConfigurationCollection.GetUpgradeCost(data.Type, data.Level);
                switch (data.Type)
                {
                    case ConfigurationType.Body:
                        _airplaneCard.ChangeInteractability(
                            _currencyService.GetCurrencyByType(CurrencyType.Coin).Currency >= upgradeCost);
                        break;
                    case ConfigurationType.Engine:
                        _slingshotRopeCard.ChangeInteractability(
                            _currencyService.GetCurrencyByType(CurrencyType.Coin).Currency >= upgradeCost);
                        break;
                    case ConfigurationType.Income:
                        _currencyMultiplierCard.ChangeInteractability(
                            _currencyService.GetCurrencyByType(CurrencyType.Coin).Currency >= upgradeCost);
                        break;
                }
            }
        }

        public void Dispose()
        {
            _airplaneCard.OnUpgrade -= UpgradeAirplane;
            _slingshotRopeCard.OnUpgrade -= UpgradeSlingshotRope;
            _currencyMultiplierCard.OnUpgrade -= UpgradeCurrencyMultiplier;

            _adsConfigurationCard.OnConfigUpgrade -= UpgradeRocket;
        }
    }
}