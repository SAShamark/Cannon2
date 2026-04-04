using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Entities.Character
{
    [Serializable]
    public class AdditionalRockets
    {
        [SerializeField] private GameObject _visual;
        [SerializeField] private List<ParticleSystem> _additionRocketsVFX;
        [SerializeField] private FuelControl _fuelControl;
        public GameObject Visual => _visual;

        public List<ParticleSystem> AdditionRocketsVFX => _additionRocketsVFX;
        public FuelControl FuelControl => _fuelControl;

        public void Init(bool isUnlocked, float maxFuel = 100f, float fuelPercent = 1f)
        {
            if (isUnlocked)
            {
                _visual.SetActive(true);
                FuelControl.Init(fuelPercent, maxFuel);
            }
        }
    }
}