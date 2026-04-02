using System;
using Services;
using TMPro;
using UI.Screens.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Variants.Gameplay
{
    public class GameplayScreen : BaseScreen
    {
        [SerializeField] private FloatingJoystick _floatingJoystick;
        [SerializeField] private FuelUI _fuelUI;

        [SerializeField] private GameObject _beforeLaunch;
        [SerializeField] private GameObject _afterLaunch;

        [SerializeField] private TextMeshProUGUI _distanceText;
        [SerializeField] private Slider _distanceSlider;
        [SerializeField] private TextMeshProUGUI _distancePercentText;
        [SerializeField] private RectTransform _bestScoreIndicator;

        [SerializeField] private TextMeshProUGUI _healthText;


        public FuelUI FuelUI => _fuelUI;
        public FloatingJoystick Joystick => _floatingJoystick;
        public event Action<float> OnLaunched;

        public void Init(int bestScore = 0)
        {
            _floatingJoystick.PointerDowned += OnLaunch;

            _beforeLaunch.SetActive(true);
            _afterLaunch.SetActive(false);

            _fuelUI.StartFuelAnimation();
            
            _bestScoreIndicator.gameObject.SetActive(bestScore > 0);
            _bestScoreIndicator.anchoredPosition = new Vector2(bestScore, _bestScoreIndicator.anchoredPosition.y);
        }

        public void UpdateDistance(int distance, float progressPercent)
        {
            _distanceText.text = $"{distance} M";
            _distanceSlider.value = progressPercent / ValueConstants.PERCENTAGE;
            _distancePercentText.text = Mathf.RoundToInt(progressPercent) + "%";
        }

        private void OnDestroy()
        {
            _fuelUI.StopFuelAnimation();
            _floatingJoystick.PointerDowned -= OnLaunch;
        }

        private void OnLaunch()
        {
            _floatingJoystick.PointerDowned -= OnLaunch;

            float captured = _fuelUI.StopFuelAnimation();

            _beforeLaunch.SetActive(false);
            _afterLaunch.SetActive(true);

            OnLaunched?.Invoke(captured);
        }

        public void UpdateHealth(float value)
        {
            _healthText.text = $"Health: {Mathf.RoundToInt(value)}%";
        }
    }
}