using UnityEngine;

namespace Gameplay.Data
{
    public class ConfigurationConfig : BaseConfigurationConfig
    {
        [SerializeField] private float _baseCost = 100;
        [SerializeField] private float _growth = 1.25f;
        [SerializeField] private float _geometricMultiplier = 3.75f;
        [Range(1, 6)] [SerializeField] private int _upgradeCountToNextPart = 6;
        [SerializeField] private Sprite _sprite;

        public int UpgradeCountToNextPart => _upgradeCountToNextPart;
        public Sprite Sprite => _sprite;

        public int GetUpgradeCost(int level)
        {
            int raw = Mathf.RoundToInt(_baseCost * Mathf.Pow(_growth, level));
            return Mathf.RoundToInt(raw / 5f) * 5;
        }

        public int GetGeometricValue(int index)
        {
            if (index < 1)
            {
                return 0;
            }

            return Mathf.RoundToInt(_baseCost * Mathf.Pow(_geometricMultiplier, index - 1));
        }
    }
}