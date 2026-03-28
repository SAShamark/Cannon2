using System;
using Services.Currency;
using Services.Storage;
using UI.Managers;
using UI.Popups;
using UI.Screens;
using UI.Screens.Base;
using UI.Screens.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuScreen : BaseScreen
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _personalizationButton;
        [SerializeField] private Button _chestButton;
        [SerializeField] private Button _achievementsButton;
        [SerializeField] private GameObject _planeTickets;
        [SerializeField] private ConfigurationManager _configurationManager;

        private CurrencyService _currencyService;
        private StorageService _storageService;
        private ConfigurationDataService _configurationDataService;
        public event Action<bool> OnAirplaneUpdated;
        public event Action<bool> OnRopeUpdated;
        public event Action OnCurrencyMultiplierUpdated;
        public event Action OnRocketUpdated;
        public event Action OnStartGameplay;

        private void OnDestroy()
        {
            _configurationManager.Dispose();
            _configurationManager.OnUpgrade -= ConfigurationUpgraded;
        }

        public override void Initialize()
        {
            base.Initialize();

            _currencyService = ServicesManager.Instance.CurrencyService;
            _storageService = ServicesManager.Instance.StorageService;
            _configurationDataService = ServicesManager.Instance.ConfigurationDataService;

            AddButtonListener(_shopButton, () => ShowScreen(ScreenTypes.Shop));
            AddButtonListener(_settingsButton, () => ShowPopup(PopupTypes.Settings));
            AddButtonListener(_achievementsButton, () => ShowPopup(PopupTypes.Achievements));
            AddButtonListener(_personalizationButton, () => ShowScreen(ScreenTypes.Personalization));
            AddButtonListener(_chestButton, () => ShowScreen(ScreenTypes.Chest));
            
            _playButton.onClick.AddListener(StartGameplay);
            _configurationManager.Initialize(_configurationDataService);
            _configurationManager.OnUpgrade += ConfigurationUpgraded;
            Draw(1);
        }

        public void Draw(int level)
        {
            if (level == 1)
            {
                ActivateFirstStage();
            }
            else
            {
                ActivateSecondStage();
            }

            DrawLevelItem(level);
            _configurationManager.UpdateUI();
        }

        private void DrawLevelItem(int level)
        {
            //AirplanesData airplaneConfigAirplaneLevel =
            //    _configurationDataService.ConfigurationCollection.AirplaneConfig.AirplaneLevels[level - 1];
            //_levelItem.Draw(true,
            //    airplaneConfigAirplaneLevel.LevelAirplane,
            //    airplaneConfigAirplaneLevel.LevelName,
            //    airplaneConfigAirplaneLevel.LevelSpeed);
        }

        private void ActivateFirstStage()
        {
            _planeTickets.SetActive(false);
            _shopButton.gameObject.SetActive(false);

            bool value = _configurationDataService.GetConfigurationData(ConfigurationType.Plane).Level >= 16;
            _configurationManager.AdsConfigurationCard.gameObject.SetActive(value);
        }

        private void ActivateSecondStage()
        {
            _shopButton.gameObject.SetActive(true);
            _planeTickets.SetActive(true);
            _configurationManager.AdsConfigurationCard.gameObject.SetActive(true);
        }

        private void ConfigurationUpgraded(ConfigurationType type, bool isCycle)
        {
            switch (type)
            {
                case ConfigurationType.Plane:
                    OnAirplaneUpdated?.Invoke(isCycle);
                    if (_configurationDataService.GetConfigurationData(ConfigurationType.Plane).Level >=
                        _configurationDataService.ConfigurationCollection.RocketConfig.PlaneLevelToUnlock)
                    {
                        _configurationManager.AdsConfigurationCard.gameObject.SetActive(true);
                    }

                    break;
                case ConfigurationType.Slingshot:
                    OnRopeUpdated?.Invoke(isCycle);
                    break;
                case ConfigurationType.Income:
                    OnCurrencyMultiplierUpdated?.Invoke();
                    break;
                case ConfigurationType.Rocket:
                    OnRocketUpdated?.Invoke();
                    break;
            }
        }

        private void StartGameplay()
        {
            ShowScreen(ScreenTypes.Gameplay);
            OnStartGameplay?.Invoke();
            ButtonClickedSound();
        }

        private void AddButtonListener(Button button, Action onClickAction)
        {
            if (button != null)
            {
                button.onClick.AddListener(onClickAction.Invoke);
            }
        }

        private void ShowScreen(ScreenTypes screenType)
        {
            UIManager.Instance.ScreensManager.ShowScreen(screenType);
            ButtonClickedSound();
        }

        private void ShowPopup(PopupTypes popupType)
        {
            UIManager.Instance.PopupsManager.ShowPopup(popupType);
            ButtonClickedSound();
        }

        public void ChangeUiActivity(bool value)
        {
            _safeAreaFitter.gameObject.SetActive(value);
        }
    }
}