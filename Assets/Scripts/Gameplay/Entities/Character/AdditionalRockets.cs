using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    [Serializable]
    public class AdditionalRockets
    {
        [SerializeField] private List<ParticleSystem> _additionRocketsVFX;
        public List<ParticleSystem> AdditionRocketsVFX => _additionRocketsVFX;
        public FuelControl FuelControl { get; private set; } = new();

        public void Init(float fuelValue)
        {
            FuelControl.Init(fuelValue);
        }
    }
}