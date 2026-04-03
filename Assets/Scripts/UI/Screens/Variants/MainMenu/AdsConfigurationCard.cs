using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Variants.MainMenu
{
    public class AdsConfigurationCard : MonoBehaviour
    {
        [SerializeField] private ConfigurationType _configurationType;
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _levelPanel;
        [SerializeField] private TMP_Text _levelText;

        private int _level;
        public event Action<ConfigurationType, int> OnConfigUpgrade;

        public void Initialize(Sprite sprite, int level)
        {
            _button.onClick.AddListener(AdsToUpgradeClicked);

            Draw(sprite, level);
        }

        public void Draw(Sprite sprite, int level)
        {
            _image.sprite = sprite;
            _level = level;
            _levelPanel.SetActive(level > 0);
        }

        private void AdsToUpgradeClicked()
        {
            //todo: Implement ad watching logic here
            if (_level < 3)
            {
                OnConfigUpgrade?.Invoke(_configurationType, _level);
            }
        }
    }
}