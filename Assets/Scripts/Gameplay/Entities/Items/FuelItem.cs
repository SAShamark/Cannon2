using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class FuelItem : BaseItem, ICatcheable
    {
        [SerializeField] private int _fuel;

        public void Catch(ICatchHandler catchHandler)
        {
            catchHandler.EarnFuel(_fuel);
            TryGetComponent(out BasePoolDestroyable poolDestroyable);
            poolDestroyable?.DestroyObject();

        }
    }
}