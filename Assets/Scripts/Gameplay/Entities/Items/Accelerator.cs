using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class Accelerator : BaseEntity, ICatcheable
    {
        [SerializeField] private float _percent;
        [SerializeField] private float _duration;

        public void Catch(ICatchHandler catchHandler)
        {
            catchHandler.GetAccelerator(_percent, _duration);
        }
    }
}