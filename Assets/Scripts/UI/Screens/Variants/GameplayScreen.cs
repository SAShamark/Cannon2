using System;
using TMPro;
using UI.Screens.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Variants
{
    public class GameplayScreen : BaseScreen
    {
        // ── Inspector ──────────────────────────────────────────────────────────
        [Header("Джойстик")]
        [SerializeField] private FloatingJoystick _flightJoystick;

        [Header("До запуску")]
        [SerializeField] private GameObject _beforeLaunch;
        [SerializeField] private Button     _startButton;

        [Header("Після запуску")]
        [SerializeField] private GameObject _afterLaunch;
        [SerializeField] private GameObject _launchBlocker;
        [SerializeField] private GameObject _currency;

        [Header("HUD дистанції")]
        [SerializeField] private Slider    _distanceSlider;
        [SerializeField] private TMP_Text  _distanceText;
        [SerializeField] private TMP_Text  _distancePercentText;
        [SerializeField] private TMP_Text  _speedText;

        [Header("Шкала палива (передстартова)")]
        [SerializeField] private Image    _fuelFillImage;
        [SerializeField] private TMP_Text _fuelPercentText;

        [Header("Анімація шкали палива")]
        [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _minSpeed = 0.2f;
        [SerializeField] private float _maxSpeed = 3f;

        [Header("Ракетний бустер (необов'язково)")]
        [SerializeField] private Button _rocketButton;
        [SerializeField] private Image  _rocketDurationImage;

        // ── Стан ──────────────────────────────────────────────────────────────
        private float _fuelValue;
        private bool  _forward   = true;
        private bool  _animating = false;   // ПОЧИНАЄ false — вмикається тільки в Init()
        private bool  _launched  = false;

        private float _rocketTimer;
        private bool  _isRocketActive;
        private int   _currentLevel;
        private float _durationByLevel;

        // ── Події ─────────────────────────────────────────────────────────────
        public event Action<float> OnLaunched;
        public event Action        OnRocketButtonClicked;

        // ── Init (викликається з GameManager) ─────────────────────────────────
        public void Init()
        {
            _launched       = false;
            _isRocketActive = false;

            _startButton.onClick.AddListener(OnLaunchButtonClicked);
            _startButton.interactable = true;

            if (_rocketDurationImage != null)
                _rocketDurationImage.fillAmount = 1f;

            if (_rocketButton != null)
                _rocketButton.onClick.AddListener(ActivateRockets);

            if (_currency != null)
                _currency.SetActive(_currentLevel > 1);

            // Показуємо передстартовий екран і запускаємо анімацію шкали
            _beforeLaunch.SetActive(true);
            _afterLaunch.SetActive(false);
            StartFuelAnimation();
        }

        // ── Unity lifecycle ────────────────────────────────────────────────────
        // Start() навмисно НЕ викликає BeforeLaunch() / StartFuelAnimation() —
        // цим керує тільки Init() щоб уникнути повторного запуску анімації.

        private void Update()
        {
            // Анімація шкали крутиться ТІЛЬКИ якщо _animating == true.
            // StopFuelAnimation() встановлює _animating = false → Update більше
            // не оновлює fillAmount → шкала заморожується на зафіксованому значенні.
            if (_animating)
                TickFuelAnimation();

            if (_isRocketActive)
                TickRocketTimer();
        }

        // ── Кнопка LAUNCH ─────────────────────────────────────────────────────
        private void OnLaunchButtonClicked()
        {
            if (_launched) return;
            _launched = true;

            // 1. Зупиняємо анімацію — _fuelValue більше не змінюється
            StopFuelAnimation();

            // 2. Блокуємо кнопку від повторного тапу
            _startButton.interactable = false;

            // 3. Фіксуємо значення і передаємо назовні
            float captured = _fuelValue;

            // 4. Перемикаємо UI
            _beforeLaunch.SetActive(false);
            _afterLaunch.SetActive(true);

            // 5. Кидаємо подію — GameManager передасть у CharacterControl
            OnLaunched?.Invoke(captured);
        }

        // ── Анімація шкали ────────────────────────────────────────────────────
        private void StartFuelAnimation()
        {
            _fuelValue = 0f;
            _forward   = true;
            _animating = true;
            UpdateFuelUI(0f);
        }

        private void StopFuelAnimation()
        {
            // Просто вимикаємо прапор — Update перестає викликати TickFuelAnimation().
            // Поточне значення _fuelValue залишається незмінним.
            _animating = false;
        }

        private void TickFuelAnimation()
        {
            float curveValue = _speedCurve.Evaluate(_fuelValue);
            float speed      = Mathf.Lerp(_minSpeed, _maxSpeed, curveValue);
            float direction  = _forward ? 1f : -1f;

            _fuelValue += direction * speed * Time.deltaTime;

            if (_fuelValue >= 1f) { _fuelValue = 1f; _forward = false; }
            if (_fuelValue <= 0f) { _fuelValue = 0f; _forward = true;  }

            UpdateFuelUI(_fuelValue);
        }

        private void UpdateFuelUI(float value)
        {
            if (_fuelFillImage != null)
                _fuelFillImage.fillAmount = value;

            if (_fuelPercentText != null)
                _fuelPercentText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }

        // ── HUD дистанції ─────────────────────────────────────────────────────
        public void UpdateDistanceHUD(float current, float max, float speed)
        {
            float normalized = max > 0f ? current / max : 0f;

            if (_distanceSlider != null)
                _distanceSlider.value = normalized;

            if (_distanceText != null)
                _distanceText.text = Mathf.RoundToInt(current) + " m";

            if (_distancePercentText != null)
                _distancePercentText.text = Mathf.RoundToInt(normalized * 100f) + "%";

            if (_speedText != null)
                _speedText.text = Mathf.RoundToInt(speed) + " km/h";
        }

        // ── Ракетний бустер ───────────────────────────────────────────────────
        private void ActivateRockets()
        {
            if (_isRocketActive) return;
            OnRocketButtonClicked?.Invoke();
            _rocketTimer = _durationByLevel;
            if (_rocketDurationImage != null)
                _rocketDurationImage.fillAmount = 1f;
            _isRocketActive = true;
        }

        private void TickRocketTimer()
        {
            _rocketTimer -= Time.deltaTime;
            if (_rocketDurationImage != null)
            {
                _rocketDurationImage.fillAmount = _durationByLevel > 0f
                    ? Mathf.Clamp01(_rocketTimer / _durationByLevel)
                    : 0f;
            }
            if (_rocketTimer <= 0f)
                _isRocketActive = false;
        }
    }
}