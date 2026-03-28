using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Data
{
    [CreateAssetMenu(fileName = "Rocket", menuName = "ScriptableObjects/Configuration/Rocket")]
    public class RocketConfig : BaseConfigurationConfig
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _planeLevelToUnlock = 16;
        [SerializeField] private float _startTrust = 2f;
        [SerializeField] private float _trustMultiplier = 0.2f;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _durationMultiplier = 0.5f;
        
        public Sprite Sprite => _sprite;
        public int PlaneLevelToUnlock => _planeLevelToUnlock;
        
        public float GetThrustByLevel(int level)
        {
            if (level <= 0) level = 1;
            return _startTrust + (level - 1) * _trustMultiplier; 
        }

        public float GetDurationByLevel(int level)
        {
            if (level <= 0) level = 1;
            return _duration + (level - 1) * _durationMultiplier; 
        }
    }
}