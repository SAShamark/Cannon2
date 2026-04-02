using System;
using Gameplay.Entities.Items;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public class SpawnEntityData
    {
        [SerializeField] private BaseEntity _entity;
        [SerializeField] private float _spawnChance;
        [SerializeField] private float _minRange;
        [SerializeField] private float _maxRange;
        [SerializeField] private int _initialPoolSize = 5;

        public BaseEntity Entity => _entity;
        public float SpawnChance => _spawnChance;
        public float MinRange => _minRange;
        public float MaxRange => _maxRange;
        public int InitialPoolSize => _initialPoolSize;
    }
}