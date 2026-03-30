using Gameplay.Entities.Character;
using UI.Screens.Variants.Gameplay;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CharacterControl _characterControl;
        [SerializeField] private GameplayScreen _gameplayScreen;
        [SerializeField] private BackgroundController _backgroundController;

        private void Start()
        {
            _backgroundController.Init(_characterControl.transform);
            _gameplayScreen.Init();
            _gameplayScreen.OnLaunched += CharacterLaunched;
        }

        private void CharacterLaunched(float fuelPercent)
        {
            _characterControl.Initialize(_gameplayScreen.Joystick, fuelPercent);
            _characterControl.FuelControl.OnFuelChanged += _gameplayScreen.FuelUI.UpdateMainDisplay;
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged +=
                _gameplayScreen.FuelUI.UpdateAdditionalDisplay;
        }

        private void OnDestroy()
        {
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged -= _gameplayScreen.FuelUI.UpdateMainDisplay;
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged -=
                _gameplayScreen.FuelUI.UpdateAdditionalDisplay;
        }
    }
}