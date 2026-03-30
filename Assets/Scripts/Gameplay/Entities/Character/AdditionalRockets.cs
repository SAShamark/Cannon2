using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    [Serializable]
    public class AdditionalRockets
    {
        [SerializeField] private List<ParticleSystem> _additionRocketsVFX;
        [SerializeField] private FuelControl _fuelControl;
        
        public List<ParticleSystem> AdditionRocketsVFX => _additionRocketsVFX;
        public FuelControl FuelControl => _fuelControl;
        public void Init(float fuelPercent)
        {
            FuelControl.Init(fuelPercent);
        }
    }
}