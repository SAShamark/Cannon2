using System;
using Gameplay.Entities.Background;
using Gameplay.Entities.Character;
using Gameplay.Entities.Spawn;
using Services.Storage;
using UI;
using UI.Managers;
using UI.Popups;
using UI.Popups.Variables;
using UI.Screens;
using UI.Screens.Variants.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    [Serializable]
    public class GameplayManager
    {
        [SerializeField] private Spawner _spawner;
        [SerializeField] private GameObject _bestScoreLine;

        private CharacterControl _characterControl;
        private float _finishLineDistance;
        private Vector3 _launchPosition;
        private float _maxDistance;
        private GameplayScreen _gameplayScreen;
        private UIManager _uiManager;
        private EnvironmentController _environmentController;

        public void Init(EnvironmentController environmentController, CharacterControl characterControl)
        {
            _environmentController = environmentController;
            _characterControl = characterControl;

            _environmentController.OnFinishReached += LevelCompleted;
            _finishLineDistance = _environmentController.GetDistanceToFinishLine();
            _spawner.Init(_characterControl.transform);

            _uiManager = UIManager.Instance;
            _gameplayScreen = _uiManager.ScreensManager.GetScreen(ScreenTypes.Gameplay) as GameplayScreen;
            _gameplayScreen.Init(PlayerPrefs.GetFloat(StorageConstants.DISTANCE, 0), _finishLineDistance);
            _gameplayScreen.OnLaunched -= CharacterLaunched;
            _gameplayScreen.OnLaunched += CharacterLaunched;
        }


        public void Update()
        {
            if (_characterControl != null && _characterControl.IsLaunched)
            {
                UpdateDistanceUI();

                if (_characterControl.transform.position.y > _bestScoreLine.transform.position.y)
                {
                    _bestScoreLine.SetActive(false);
                }
                else
                {
                    _bestScoreLine.transform.position = new Vector2(_characterControl.transform.position.x,
                        _bestScoreLine.transform.position.y);
                }
            }
        }

        public void OnDestroy()
        {
            _environmentController.OnFinishReached -= LevelCompleted;
            if (_characterControl != null && _characterControl.IsLaunched)
            {
                _characterControl.FuelControl.OnFuelChanged -=
                    _gameplayScreen.FuelUI.UpdateMainDisplay;
                _characterControl.AdditionalRockets.FuelControl.OnFuelChanged -=
                    _gameplayScreen.FuelUI.UpdateAdditionalDisplay;
                _characterControl.HealthControl.OnDied -= CharacterDied;
                _characterControl.HealthControl.OnHealthChanged -= _gameplayScreen.UpdateHealth;
                _characterControl.OnLevelFinished -= LevelCompleted;
                _gameplayScreen.OnLaunched -= CharacterLaunched;
            }
        }

        private void CharacterLaunched(float fuelPercent)
        {
            _characterControl.HealthControl.OnHealthChanged += _gameplayScreen.UpdateHealth;
            _characterControl.Initialize(_gameplayScreen.Joystick, fuelPercent);
            _characterControl.FuelControl.OnFuelChanged += _gameplayScreen.FuelUI.UpdateMainDisplay;
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged +=
                _gameplayScreen.FuelUI.UpdateAdditionalDisplay;
            _characterControl.HealthControl.OnDied += CharacterDied;
            _characterControl.OnLevelFinished += LevelCompleted;

            _launchPosition = _characterControl.transform.position;

            if (PlayerPrefs.HasKey(StorageConstants.DISTANCE) && PlayerPrefs.GetFloat(StorageConstants.DISTANCE) > 0f)
            {
                _bestScoreLine.gameObject.SetActive(true);
                var f = PlayerPrefs.GetFloat(StorageConstants.DISTANCE);
                _bestScoreLine.transform.position = new Vector2(0, f);
            }
            else
            {
                _bestScoreLine.gameObject.SetActive(false);
            }
        }

        private void LevelCompleted()
        {
            _uiManager.PopupsManager.ShowPopup(PopupTypes.LevelComplete);
            var popup = _uiManager.PopupsManager.GetPopup(PopupTypes.LevelComplete) as LevelCompletedPopup;
            popup.Init();
        }

        private void CharacterDied()
        {
            float bestScore = PlayerPrefs.GetFloat(StorageConstants.DISTANCE, 0f);
            if (_maxDistance > bestScore)
            {
                PlayerPrefs.SetFloat(StorageConstants.DISTANCE, _maxDistance);
            }

            MainMenuState();
        }

        private void MainMenuState()
        {
            _characterControl = null;
            _gameplayScreen.BeforeLaunch();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void UpdateDistanceUI()
        {
            float distance = _characterControl.transform.position.y - _launchPosition.y;
            distance = Mathf.Max(0f, distance);

            if (distance > _maxDistance)
                _maxDistance = distance;

            float progressPercent = _finishLineDistance > 0f
                ? Mathf.Clamp01(distance / _finishLineDistance) * 100f
                : 0f;

            _gameplayScreen.UpdateDistance(Mathf.RoundToInt(distance), progressPercent);
        }
    }
}