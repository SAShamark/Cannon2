using System;
using UI.Screens.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Variants.Gameplay
{
    public class GameplayScreen : BaseScreen
    {
        [SerializeField] private FloatingJoystick _floatingJoystick;

        [SerializeField] private GameObject _beforeLaunch;
        [SerializeField] private Button _startButton;

        [SerializeField] private GameObject _afterLaunch;
        [SerializeField] private GameObject _launchBlocker;

        [SerializeField] private FuelUI _fuelUI;


        public FuelUI FuelUI => _fuelUI;
        public FloatingJoystick Joystick => _floatingJoystick;
        public event Action<float> OnLaunched;

        public void Init()
        {
            _startButton.onClick.AddListener(OnLaunchButtonClicked);
            _startButton.interactable = true;

            _beforeLaunch.SetActive(true);
            _afterLaunch.SetActive(false);

            _fuelUI.StartFuelAnimation();
        }

        private void OnDestroy()
        {
            _fuelUI.StopFuelAnimation();
            _startButton.onClick.RemoveListener(OnLaunchButtonClicked);
        }

        private void OnLaunchButtonClicked()
        {
            float captured = _fuelUI.StopFuelAnimation();

            _beforeLaunch.SetActive(false);
            _afterLaunch.SetActive(true);

            OnLaunched?.Invoke(captured);
        }
    }
}