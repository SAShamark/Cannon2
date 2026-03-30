using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class CrystalItem : BaseItem
    {
        [SerializeField] private float _count;

        public float Count => _count;
    }
}