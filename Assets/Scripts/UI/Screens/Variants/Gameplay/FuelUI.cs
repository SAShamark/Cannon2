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
        [SerializeField] private FuelDisplay _mainDisplay;
        [SerializeField] private FuelDisplay _additionalDisplay;

        [SerializeField] private AnimationCurve _forwardCurve = new(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(1f, 1f, 2f, 2f)
        );

        [SerializeField] private AnimationCurve _backwardCurve = new(
            new Keyframe(0f, 0f, 2f, 2f),
            new Keyframe(1f, 1f, 0f, 0f)
        );

        [SerializeField] private float _forwardDuration = 2f;
        [SerializeField] private float _backwardDuration = 1.5f;

        public float FuelValue { get; private set; }

        private Tween _tween;

        public void StartFuelAnimation()
        {
            FuelValue = 0f;
            _mainDisplay.UpdateDisplay(0f);
            AnimateForward();
        }

        public float StopFuelAnimation()
        {
            _tween?.Kill();
            return FuelValue;
        }

        public void UpdateAdditionalDisplay(float value) =>
            _additionalDisplay.UpdateDisplay(value);
        public void UpdateMainDisplay(float value) =>
            _mainDisplay.UpdateDisplay(value);
        private void AnimateForward()
        {
            _tween = DOTween
                .To(() => FuelValue, SetFuelValue, 1f, _forwardDuration)
                .SetEase(_forwardCurve)
                .OnComplete(AnimateBackward);
        }

        private void AnimateBackward()
        {
            _tween = DOTween
                .To(() => FuelValue, SetFuelValue, 0f, _backwardDuration)
                .SetEase(_backwardCurve)
                .OnComplete(AnimateForward);
        }

        private void SetFuelValue(float value)
        {
            FuelValue = value;
            _mainDisplay.UpdateDisplay(value);
        }
    }

    [Serializable]
    public class FuelDisplay
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _percentText;

        public void UpdateDisplay(float value)
        {
            if (_fillImage != null)
                _fillImage.fillAmount = value;

            if (_percentText != null)
                _percentText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }
}