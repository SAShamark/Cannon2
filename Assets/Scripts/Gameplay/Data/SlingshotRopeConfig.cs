    using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Data
{
    [CreateAssetMenu(fileName = "SlingshotRope", menuName = "ScriptableObjects/Configuration/SlingshotRope")]
    public class SlingshotRopeConfig : ConfigurationConfig
    {
        [SerializeField] private List<Material> _ropeMaterials;

        [Header("Launch Force")]
        [SerializeField] private float _baseLaunchForce = 1f;
        [SerializeField] private float _launchForcePerUpgrade = 0.3f;

        [Header("Cruise Speed")]
        [SerializeField] private float _baseCruiseSpeed = 1f;
        [SerializeField] private float _cruiseSpeedPerUpgrade = 0.8f;

        public List<Material> RopeMaterials => _ropeMaterials;

        public float GetLaunchForce(int upgradeLevel)
        {
            return _baseLaunchForce + _launchForcePerUpgrade * Mathf.Sqrt(upgradeLevel);
        }

        public float GetCruiseSpeedMultiplier(int upgradeLevel)
        {
            return _baseCruiseSpeed + _cruiseSpeedPerUpgrade * Mathf.Sqrt(upgradeLevel);
        }
    }
}