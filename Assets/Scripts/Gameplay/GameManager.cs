using System;
using Gameplay.Entities.Character;
using UI.Screens.Variants;
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

        private void CharacterLaunched(float fuelValue)
        {
            _characterControl.Initialize(_gameplayScreen.Joystick, fuelValue);
            _characterControl.FuelControl.OnFuelChanged += _gameplayScreen.FuelUI.UpdateFuelUI;
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged += _gameplayScreen.FuelUI.UpdateAdditionalFuelUI;
        }

        private void OnDestroy()
        {
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged -= _gameplayScreen.FuelUI.UpdateFuelUI;
            _characterControl.AdditionalRockets.FuelControl.OnFuelChanged -= _gameplayScreen.FuelUI.UpdateAdditionalFuelUI;
        }
    }
}