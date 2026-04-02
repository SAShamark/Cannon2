using System;
using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class BaseItem : BaseEntity
    {
        private ICatcheable[] _catcheables;
        public event Action<BaseItem> OnCaught;

        private void Start()
        {
            _catcheables = gameObject.GetComponents<ICatcheable>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            ProcessCollider(other);
        }

        private void ProcessCollider(Collider2D other)
        {
            if (other.attachedRigidbody)
            {
                if (other.attachedRigidbody.TryGetComponent(out ICatchHandler catchHandler))
                {
                    Catch(catchHandler);
                }
            }
        }

        private void Catch(ICatchHandler catchHandler)
        {
            OnCaught?.Invoke(this);
            foreach (ICatcheable catcheable in _catcheables)
            {
                catcheable.Catch(catchHandler);
            }
        }
    }
}