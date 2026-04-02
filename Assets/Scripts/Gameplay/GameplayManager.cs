using System;
using Gameplay.Entities.Background;
using Gameplay.Entities.Character;
using Gameplay.Entities.Spawn;
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
        private BackgroundController _backgroundController;

        public void Init(BackgroundController backgroundController, CharacterControl characterControl)
        {
            _backgroundController = backgroundController;
            _uiManager = UIManager.Instance;
            _characterControl = characterControl;

            _uiManager.ScreensManager.ShowScreen(ScreenTypes.Gameplay);
            _gameplayScreen = _uiManager.ScreensManager.GetScreen(ScreenTypes.Gameplay) as GameplayScreen;
            _gameplayScreen.Init();
            _gameplayScreen.OnLaunched += CharacterLaunched;

            _finishLineDistance = _backgroundController.GetDistanceToFinishLine();
            _spawner.Init(_characterControl.transform);
        }

        private void Update()
        {
            if (_characterControl.IsLaunched)
            {
                UpdateDistanceUI();
                _bestScoreLine.transform.position = new Vector2(_characterControl.transform.position.x,
                    _bestScoreLine.transform.position.y);
                if (_characterControl.transform.position.y > _bestScoreLine.transform.position.y)
                {
                    _bestScoreLine.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged -= _gameplayScreen.FuelUI.UpdateMainDisplay;
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged -=
                _gameplayScreen.FuelUI.UpdateAdditionalDisplay;
            _characterControl.HealthControl.OnDied -= CharacterDied;
            _characterControl.HealthControl.OnHealthChanged -= _gameplayScreen.UpdateHealth;
            _characterControl.OnLevelFinished -= LevelCompleted;
        }

        private void CharacterLaunched(float fuelPercent)
        {
            _characterControl.Initialize(_gameplayScreen.Joystick, fuelPercent);
            _characterControl.FuelControl.OnFuelChanged += _gameplayScreen.FuelUI.UpdateMainDisplay;
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged +=
                _gameplayScreen.FuelUI.UpdateAdditionalDisplay;
            _characterControl.HealthControl.OnDied += CharacterDied;
            _characterControl.HealthControl.OnHealthChanged += _gameplayScreen.UpdateHealth;
            _characterControl.OnLevelFinished += LevelCompleted;

            _launchPosition = _characterControl.transform.position;

            if (PlayerPrefs.HasKey("BestScore") && PlayerPrefs.GetFloat("BestScore") > 0f)
            {
                _bestScoreLine.gameObject.SetActive(true);
                var f = PlayerPrefs.GetFloat("BestScore");
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
            float bestScore = PlayerPrefs.GetFloat("BestScore", 0f);
            if (_maxDistance > bestScore)
            {
                PlayerPrefs.SetFloat("BestScore", _maxDistance);
            }

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