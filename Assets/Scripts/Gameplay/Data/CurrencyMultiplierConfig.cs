using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Data
{
    [CreateAssetMenu(fileName = "CurrencyMultiplier", menuName = "ScriptableObjects/Configuration/CurrencyMultiplier")]
    public class CurrencyMultiplierConfig : ConfigurationConfig
    {
        [SerializeField] private List<Sprite> _sprites;

        public List<Sprite> Sprites => _sprites;

        public float GetUpgradeMultiplier(int incomeLevel, int gameLevel)
        {
            if (incomeLevel < 1)
                return 0f;

            int step = (incomeLevel - 1) / 5;
            float baseValue = Mathf.Pow(2, step);
            int offset = (incomeLevel - 1) % 5;
            float increment = baseValue / 10f;

            float baseMultiplier = baseValue + increment * offset;

            float levelMultiplier = Mathf.Pow(2f, gameLevel - 1);
            return baseMultiplier * levelMultiplier;
        }
    }
}