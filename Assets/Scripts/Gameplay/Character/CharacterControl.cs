using UnityEngine;

namespace Gameplay.Character
{
    /// <summary>
    /// Керує ракетою після запуску в стилі WONDER ROCKET.
    ///
    /// Модель руху:
    ///   • Ракета ЗАВЖДИ летить вгору з постійною вертикальною швидкістю.
    ///   • Джойстик лише відхиляє горизонтально — ракета не може летіти вниз чи стояти.
    ///   • Горизонтальна швидкість = joystick.x × maxSideSpeed (з розгоном/гальмуванням).
    ///   • Вертикальна швидкість = baseUpSpeed × fuelMultiplier × FuelControl.SpeedMultiplier.
    ///   • При закінченні палива: корабель плавно втрачає підйомну силу і починає падати.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterControl : MonoBehaviour
    {
        // ── Inspector ──────────────────────────────────────────────────────────
        [Header("Керування")]
        [SerializeField] private FloatingJoystick _floatingJoystick;

        [Header("Вертикальний рух (вгору)")]
        [Tooltip("Базова швидкість підйому (world units/s) при fuelMultiplier=1.0")]
        [SerializeField] private float _baseUpSpeed      = 6f;

        [Header("Горизонтальний рух (джойстик)")]
        [Tooltip("Максимальна бокова швидкість")]
        [SerializeField] private float _maxSideSpeed     = 5f;
        [Tooltip("Швидкість розгону по горизонталі")]
        [SerializeField] private float _sideAcceleration = 8f;
        [Tooltip("Швидкість гальмування по горизонталі (менше = плавучіше)")]
        [SerializeField] private float _sideDeceleration = 5f;

        [Header("Нахил корабля (косметичний)")]
        [Tooltip("Максимальний кут нахилу по Z при повному боковому відхиленні")]
        [SerializeField] private float _maxTiltAngle     = 25f;
        [Tooltip("Швидкість повороту нахилу")]
        [SerializeField] private float _tiltSpeed        = 8f;

        [Header("Падіння при закінченні палива")]
        [Tooltip("Сила гравітації при падінні")]
        [SerializeField] private float _fallGravityScale = 2f;
        [Tooltip("Час плавного переходу від підйому до падіння (секунди)")]
        [SerializeField] private float _fallTransition   = 1.5f;

        [Header("Межі екрана")]
        [SerializeField] private bool  _clampToScreen    = true;
        [Tooltip("Відступ від краю (world units)")]
        [SerializeField] private float _screenPadding    = 0.5f;

        [Header("Стартовий імпульс")]
        [Tooltip("Тривалість додаткового вертикального прискорення при старті")]
        [SerializeField] private float _boostDuration    = 0.8f;
        [Tooltip("Додаткова сила імпульсу при заряді 100%")]
        [SerializeField] private float _boostForce       = 20f;

        [Header("VFX (необов'язково)")]
        [SerializeField] private ParticleSystem _thrustParticles;
        [SerializeField] private TrailRenderer  _trail;

        [Header("HUD палива під час польоту (необов'язково)")]
        [SerializeField] private UnityEngine.UI.Image  _hudFuelBar;
        [SerializeField] private TMPro.TextMeshProUGUI _hudFuelLabel;

        // ── Публічні властивості ───────────────────────────────────────────────
        public FloatingJoystick Joystick    => _floatingJoystick;
        public FuelControl      FuelControl { get; private set; }

        // ── Приватний стан ─────────────────────────────────────────────────────
        private Rigidbody2D       _rb;
        private Camera            _cam;
        private Vector2           _screenMin;
        private Vector2           _screenMax;
        private ScreenOrientation _lastOrientation;

        private bool  _isLaunched;
        private float _fuelMultiplier;        // зафіксований при старті (0.4..1.5)
        private float _boostTimer;            // залишок часу стартового імпульсу

        private float _currentSideVelocity;   // тільки горизонталь, плавна
        private float _upVelocity;            // вертикальна швидкість, керована вручну

        // Стан падіння
        private bool  _isFalling;
        private float _fallTimer;             // скільки часу пройшло після закінчення палива

        // ── Unity lifecycle ────────────────────────────────────────────────────
        private void Awake()
        {
            _rb  = GetComponent<Rigidbody2D>();
            _cam = Camera.main;

            FuelControl = new FuelControl();
        }

        private void Start()
        {
            RecalculateScreenBounds();
        }

        private void Update()
        {
            UpdateScreenBounds();

            if (!_isLaunched) return;

            // Витрата палива
            float throttle = Mathf.Clamp01(Mathf.Abs(_currentSideVelocity) / _maxSideSpeed);
            bool  fuelOk   = FuelControl.Tick(throttle, Time.deltaTime);

            UpdateHUD();

            // Паливо скінчилось — починаємо перехід до падіння
            if (!fuelOk && !_isFalling)
                StartFalling();
        }

        private void FixedUpdate()
        {
            if (!_isLaunched) return;

            if (_isFalling)
            {
                HandleFalling();
            }
            else
            {
                HandleFlying();
            }

            // ВАЖЛИВО: Викликаємо Clamp після розрахунку швидкості, 
            // але перед тим як фізичний двіжок відрендерить кадр
            ClampToScreen();
        }

        // ── Публічне API ───────────────────────────────────────────────────────

        /// <summary>Викликається з GameManager після OnLaunched(fuelValue).</summary>
        public void Initialize(float fuelValue)
        {
            FuelControl.Start(fuelValue);

            _fuelMultiplier       = Mathf.Lerp(0.4f, 1.5f, fuelValue);
            _boostTimer           = _boostDuration;
            _currentSideVelocity  = 0f;
            _upVelocity           = 0f;
            _isFalling            = false;
            _fallTimer            = 0f;
            _isLaunched           = true;

            _rb.gravityScale = 0f;

        }

        // ── Рух під час польоту ────────────────────────────────────────────────
        private void HandleFlying()
        {
            float joystickX = _floatingJoystick != null ? _floatingJoystick.Horizontal : 0f;

            // ── Горизонталь: джойстик скеровує, ракета розганяється і гальмує ──
            float targetSide = joystickX * _maxSideSpeed;
            float lerpFactor = Mathf.Abs(joystickX) > 0.05f ? _sideAcceleration : _sideDeceleration;
            _currentSideVelocity = Mathf.Lerp(_currentSideVelocity, targetSide, lerpFactor * Time.fixedDeltaTime);

            // ── Вертикаль: постійний підйом вгору ─────────────────────────────
            float baseUp     = _baseUpSpeed * _fuelMultiplier * FuelControl.SpeedMultiplier;
            _upVelocity      = baseUp;

            // Стартовий імпульс додається поверх базового підйому
            if (_boostTimer > 0f)
            {
                float boostT  = _boostTimer / _boostDuration;   // 1 → 0
                float boost   = _boostForce * _fuelMultiplier * boostT;
                _rb.AddForce(Vector2.up * boost, ForceMode2D.Force);
                _boostTimer  -= Time.fixedDeltaTime;
            }

            // ── Задаємо фінальну швидкість ────────────────────────────────────
            _rb.linearVelocity = new Vector2(_currentSideVelocity, _upVelocity);

            // ── Нахил за горизонтальним рухом ────────────────────────────────
            TiltShip(joystickX);

        }

        // ── Падіння після закінчення палива ───────────────────────────────────
        private void StartFalling()
        {
            _isFalling = true;
            _fallTimer = 0f;
        }

        private void HandleFalling()
        {
            _fallTimer += Time.fixedDeltaTime;

            // Плавний перехід: підйомна сила зменшується до нуля за _fallTransition секунд
            float fallBlend = Mathf.Clamp01(_fallTimer / _fallTransition);

            float joystickX          = _floatingJoystick != null ? _floatingJoystick.Horizontal : 0f;
            float targetSide         = joystickX * _maxSideSpeed * 0.5f;   // обмежена бокова у падінні
            _currentSideVelocity     = Mathf.Lerp(_currentSideVelocity, targetSide, _sideDeceleration * Time.fixedDeltaTime);

            // Вертикаль: від поточного підйому → до падіння через гравітацію
            float upContribution     = Mathf.Lerp(_upVelocity, 0f, fallBlend);
            float gravityContribution= -_fallGravityScale * 9.81f * fallBlend * _fallTimer;

            _rb.linearVelocity = new Vector2(
                _currentSideVelocity,
                upContribution + gravityContribution);

            // Нахил вирівнюється при падінні
            TiltShip(joystickX * (1f - fallBlend));
        }

        // ── Нахил ─────────────────────────────────────────────────────────────
        private void TiltShip(float sideInput)
        {
            float targetAngle  = -sideInput * _maxTiltAngle;
    
            // Використовуйте MoveRotation для фізичних об'єктів
            float currentAngle = _rb.rotation; 
            float nextAngle = Mathf.LerpAngle(currentAngle, targetAngle, _tiltSpeed * Time.fixedDeltaTime);
    
            _rb.MoveRotation(nextAngle);
        }

        // ── Межі екрана ───────────────────────────────────────────────────────
        private void ClampToScreen()
        {
            if (!_clampToScreen) return;

            // Використовуємо _rb.position замість transform.position
            Vector2 pos = _rb.position; 
    
            pos.x = Mathf.Clamp(pos.x, _screenMin.x, _screenMax.x);
            if (pos.y < _screenMin.y) pos.y = _screenMin.y;

            // Записуємо назад у Rigidbody, щоб не збивати інтерполяцію
            _rb.position = pos; 
        }

    

        private void RecalculateScreenBounds()
        {
            if (_cam == null) return;
            float h = _cam.orthographicSize;
            float w = h * _cam.aspect;
            _screenMin = new Vector2(-w + _screenPadding, -h + _screenPadding);
            _screenMax = new Vector2( w - _screenPadding,  h - _screenPadding);
        }

        private void UpdateScreenBounds()
        {
            if (Screen.orientation == _lastOrientation) return;
            _lastOrientation = Screen.orientation;
            RecalculateScreenBounds();
        }

        // ── HUD ───────────────────────────────────────────────────────────────
        private void UpdateHUD()
        {
            if (_hudFuelBar != null)
                _hudFuelBar.fillAmount = FuelControl.Normalized;

            if (_hudFuelLabel != null)
                _hudFuelLabel.text = Mathf.RoundToInt(FuelControl.Normalized * 100f) + "%";
        }

        // ── Gizmos ────────────────────────────────────────────────────────────
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_clampToScreen || !Application.isPlaying) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(
                (Vector3)(_screenMin + _screenMax) * 0.5f,
                (Vector3)(_screenMax - _screenMin));
        }
#endif
    }
}