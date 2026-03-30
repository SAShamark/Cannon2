using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class FuelItem : BaseItem
    {
        [SerializeField] private int _fuel;

        public float Fuel => _fuel;
    }
}