using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class MagnetItem : BaseItem, ICatcheable
    {
        [SerializeField] private float _radius;
        [SerializeField] private float _duration;


        public void Catch(ICatchHandler catchHandler)
        {
            catchHandler.GetMagnet(_radius, _duration);
            TryGetComponent(out BasePoolDestroyable poolDestroyable);
            poolDestroyable?.DestroyObject();
        }
    }
}