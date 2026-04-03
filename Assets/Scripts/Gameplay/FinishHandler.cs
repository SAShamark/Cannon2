using System;
using UnityEngine;

namespace Gameplay
{
    public class FinishHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _finishParticle;

        private bool _hasTriggered;
        public event Action OnFinishCrossed;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasTriggered)
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                if (_finishParticle != null)
                {
                    _finishParticle.transform.position = other.transform.position;
                    _finishParticle.SetActive(true);
                }

                _hasTriggered = true;
                FinishCrossed();
            }
        }

        private void FinishCrossed()
        {
            OnFinishCrossed?.Invoke();
            Debug.Log("Finish crossed!");
        }
    }
}