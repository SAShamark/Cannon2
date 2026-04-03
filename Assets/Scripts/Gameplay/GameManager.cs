using System;
using Gameplay.Entities.Background;
using Gameplay.Entities.Character;
using Services.Storage;
using UI;
using UI.Managers;
using UI.Screens;
using UI.Screens.Variants;
using UI.Screens.Variants.Gameplay;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private EnvironmentController _environmentController;
        [SerializeField] private CharacterControl _characterControl;
        [SerializeField] private GameplayManager _gameplayManager;
        [SerializeField] private CameraControl _cameraControl;
        [SerializeField]  private Transform _characterContainer;

        private UIManager _uiManager;
        private MainMenuScreen _mainMenuScreen;
        private GameplayScreen _gameplayScreen;
        private int _currentLevel;
        private bool _isGameplayInitialized;
        private CharacterControl _characterInstance;

        private void Start()
        {
            _characterInstance = Instantiate(_characterControl, _characterContainer);
            _environmentController.Init(_characterInstance.transform);
            _cameraControl.Initialize(_characterInstance);
            _currentLevel = PlayerPrefs.GetInt(StorageConstants.LEVEL_PROGRESS_KEY, 1);
            _uiManager = UIManager.Instance;
            _uiManager.ScreensManager.OnScreenShowed += ScreenShowed;
            ShowMainMenuScreen();
            _mainMenuScreen.OnStartGameplay += GameplayState;
        }

        private void Update()
        {
            _gameplayManager.Update();
        }

        private void OnDestroy()
        {
            _gameplayManager.OnDestroy();
            _uiManager.ScreensManager.OnScreenShowed -= ScreenShowed;
            _mainMenuScreen.OnStartGameplay -= GameplayState;
        }

        private void ShowMainMenuScreen()
        {
            _uiManager.ScreensManager.ShowScreen(ScreenTypes.MainMenu);
            _mainMenuScreen = _uiManager.ScreensManager.GetScreen(ScreenTypes.MainMenu) as MainMenuScreen;
            _mainMenuScreen.Draw(_currentLevel);
        }

        private void GameplayState()
        {
            if (!_isGameplayInitialized)
            {
                _isGameplayInitialized = true;
                _gameplayManager.Init(_environmentController, _characterInstance);
            }
        }

        private void ScreenShowed(ScreenTypes screenType)
        {
            if (screenType == ScreenTypes.Gameplay)
            {
                _cameraControl.ActivateGameplayCamera();
            }
            else if (screenType == ScreenTypes.MainMenu)
            {
                _cameraControl.ActivateMainMenuCamera();
            }
        }
    }
}