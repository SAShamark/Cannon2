using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Variants.Gameplay
{
    [Serializable]
    public class FuelUI
    {
        [SerializeField] private Image _fuelFillImage;
        [SerializeField] private TextMeshProUGUI _fuelPercentText;

        [SerializeField] private Image _additionalFuelFillImage;
        [SerializeField] private TextMeshProUGUI _additionalFuelPercentText;
        
        [SerializeField] private float _minSpeed = 0.2f;
        [SerializeField] private float _maxSpeed = 3f;
        [SerializeField] private Ease _easeType = Ease.InOutSine;

        public float FuelValue { get; private set; }

        private Tween _tween;

        public void StartFuelAnimation()
        {
            FuelValue = 0f;
            UpdateFuelUI(0f);
            AnimateForward();
        }

        public float StopFuelAnimation()
        {
            _tween?.Kill();
            return FuelValue;
        }

        private void AnimateForward()
        {
            float duration = 1f / Mathf.Lerp(_minSpeed, _maxSpeed, 0f);

            _tween = DOTween
                .To(() => FuelValue, SetFuelValue, 1f, duration)
                .SetEase(_easeType)
                .OnComplete(AnimateBackward);
        }

        private void AnimateBackward()
        {
            float duration = 1f / Mathf.Lerp(_minSpeed, _maxSpeed, 1f);

            _tween = DOTween
                .To(() => FuelValue, SetFuelValue, 0f, duration)
                .SetEase(_easeType)
                .OnComplete(AnimateForward);
        }

        private void SetFuelValue(float value)
        {
            FuelValue = value;
            UpdateFuelUI(value);
            UpdateAdditionalFuelUI(1);
        }

        public void UpdateFuelUI(float value)
        {
            if (_fuelFillImage != null)
                _fuelFillImage.fillAmount = value;

            if (_fuelPercentText != null)
                _fuelPercentText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }

        public void UpdateAdditionalFuelUI(float value)
        {
            if (_additionalFuelFillImage != null)
                _additionalFuelFillImage.fillAmount = value;

            if (_additionalFuelPercentText != null)
                _additionalFuelPercentText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}