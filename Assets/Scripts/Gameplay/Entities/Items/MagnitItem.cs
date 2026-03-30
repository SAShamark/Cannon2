using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class MagnitItem : BaseItem, ICollectable
    {
        [SerializeField] private float _count;

        public float Count => _count;
    }
}