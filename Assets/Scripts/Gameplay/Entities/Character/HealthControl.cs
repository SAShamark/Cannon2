using System;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    [Serializable]
    public class HealthControl
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _minImpactSpeed = 2f;
        [SerializeField] private float _maxImpactSpeed = 15f;
        [SerializeField] private float _maxDamagePerHit = 100f;
        [SerializeField] private float _angleInfluence = 1.5f;

        public int Health { get; private set; }
        public bool IsDead => Health <= 0;

        public event Action<float> OnHealthChanged;
        public event Action OnDied;

        public void Init()
        {
            Health = _maxHealth;
            OnHealthChanged?.Invoke(Health);
        }

        public void TakeCollisionDamage(Vector2 relativeVelocity, Vector2 contactNormal)
        {
            if (IsDead) return;

            float speed = relativeVelocity.magnitude;
            if (speed < _minImpactSpeed) return;

            float impactDot = Mathf.Abs(Vector2.Dot(relativeVelocity.normalized, contactNormal));
            float angleMultiplier = Mathf.Lerp(1f / _angleInfluence, 1f, impactDot);
            float speedT = Mathf.InverseLerp(_minImpactSpeed, _maxImpactSpeed, speed);
            int damage = Mathf.RoundToInt(Mathf.Lerp(0f, _maxDamagePerHit, speedT) * angleMultiplier);

            if (damage <= 0) return;

            Health = Mathf.Max(0, Health - damage);
            OnHealthChanged?.Invoke(Health);

            if (IsDead)
                OnDied?.Invoke();
        }

        public void Heal(int amount)
        {
            if (IsDead) return;

            Health = Mathf.Min(_maxHealth, Health + amount);
            OnHealthChanged?.Invoke(Health);
        }
    }
}