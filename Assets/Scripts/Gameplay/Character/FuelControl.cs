using UnityEngine;

namespace Gameplay.Character
{
    /// <summary>
    /// Чиста C#-клас (не MonoBehaviour) — керує паливом під час польоту.
    ///
    /// Створюється в CharacterControl.Initialize().
    /// Tick() викликається з CharacterControl.Update().
    /// </summary>
    public class FuelControl
    {
        // ── Налаштування ───────────────────────────────────────────────────────
        // Скільки секунд летить корабель при заряді 1.0 (100%)
        private const float MaxFuelDuration = 30f;

        // Витрата палива коли корабель стоїть (множник)
        private const float IdleDrain = 0.3f;

        // Витрата палива при повному газі (множник)
        private const float MoveDrain = 1.0f;

        // Множник швидкості при 0% палива
        private const float MinSpeedMult = 0.4f;

        // Множник швидкості при 100% палива
        private const float MaxSpeedMult = 1.5f;

        // ── Стан ──────────────────────────────────────────────────────────────
        private float _totalSeconds;      // загальний запас секунд для цього запуску
        private float _remaining;         // залишок секунд
        private bool  _isActive;

        // ── Публічні властивості ───────────────────────────────────────────────

        /// <summary>Поточний рівень палива 0..1.</summary>
        public float Normalized { get; private set; }

        /// <summary>Множник швидкості для руху корабля (0.4..1.5).</summary>
        public float SpeedMultiplier =>
            Mathf.Lerp(MinSpeedMult, MaxSpeedMult, Normalized);

        public bool IsActive => _isActive;

        // ── Ініціалізація ──────────────────────────────────────────────────────

        /// <summary>
        /// Викликається з CharacterControl.Initialize().
        /// fuelValue: 0..1 — рівень зафіксованої шкали при запуску.
        /// </summary>
        public void Start(float fuelValue)
        {
            _totalSeconds = MaxFuelDuration * fuelValue;
            _remaining    = _totalSeconds;
            Normalized    = fuelValue;
            _isActive     = true;
        }

        // ── Tick (викликається щокадру з CharacterControl) ────────────────────

        /// <summary>
        /// Оновлює залишок палива.
        /// throttle: 0..1 — нормалізована швидкість корабля.
        /// Повертає false коли паливо скінчилось.
        /// </summary>
        public bool Tick(float throttle, float deltaTime)
        {
            if (!_isActive) return false;

            float drain  = Mathf.Lerp(IdleDrain, MoveDrain, throttle);
            _remaining   = Mathf.Max(_remaining - deltaTime * drain, 0f);
            Normalized   = _totalSeconds > 0f ? _remaining / _totalSeconds : 0f;

            if (_remaining <= 0f)
            {
                _isActive = false;
                return false;
            }

            return true;
        }
    }
}