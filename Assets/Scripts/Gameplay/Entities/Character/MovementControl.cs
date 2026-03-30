using System;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    [Serializable]
    public class MovementControl
    {
        [SerializeField] private float _maxVerticalSpeed = 20f;
        [SerializeField] private float _maxHorizontalSpeed = 10f;
        [SerializeField] private float _tiltSensitivity = 90f;
        [SerializeField] private float _thrustForce = 12f;
        [SerializeField] private float _maxTotalTilt = 60f;

        public float MaxSpeed => _maxVerticalSpeed;
        public void ApplyMovement(Rigidbody2D rb, Vector2 joystickInput, bool canMove)
        {
            if (!canMove)
            {
                rb.angularVelocity = 0f;
                return;
            }

            Rotation(rb, joystickInput);

           
            float throttle = 1f + joystickInput.y; 
            throttle = Mathf.Max(0f, throttle);

            Vector2 thrustDirection = rb.transform.up;
            rb.AddForce(thrustDirection * (_thrustForce * throttle), ForceMode2D.Force);

            rb.linearVelocity = new Vector2(
                Mathf.Clamp(rb.linearVelocity.x, -_maxHorizontalSpeed, _maxHorizontalSpeed),
                Mathf.Clamp(rb.linearVelocity.y, -12f, _maxVerticalSpeed)
            );
        }

        private void Rotation(Rigidbody2D rb, Vector2 joystickInput)
        {
            float rotationAmount = -joystickInput.x * _tiltSensitivity * Time.fixedDeltaTime;
            float newAngle = rb.rotation + rotationAmount;
            newAngle = Mathf.Clamp(newAngle, -_maxTotalTilt, _maxTotalTilt);
            rb.MoveRotation(newAngle);
        }
    }
}