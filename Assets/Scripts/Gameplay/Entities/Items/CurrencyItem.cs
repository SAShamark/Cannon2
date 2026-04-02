using Services.Currency;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class CurrencyItem : BaseItem, ICatcheable
    {
        [SerializeField] private int _count;
        [SerializeField] private CurrencyType _type;

        public void Catch(ICatchHandler catchHandler)
        {
            catchHandler.EarnCurrency(_type, _count);
            TryGetComponent(out BasePoolDestroyable poolDestroyable);
            poolDestroyable?.DestroyObject();
        }
    }
}