using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ConfigurationCard : MonoBehaviour
    {
        [SerializeField] private ConfigurationType _configurationType;
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private List<Image> _levelIndicators;
        [SerializeField] private Sprite _filledIndicatorSprite;
        [SerializeField] private Sprite _emptyIndicatorSprite;
        [SerializeField] private ConfigurationManager _configurationManager;

        private int _level;
        private int _indicatorsCountToNextPart;
        public event Action OnUpgrade;
        
        public void Initialize(Sprite sprite, int indicatorsCountToNextPart, int level, int cost, float value = 0f)
        {
            _indicatorsCountToNextPart = indicatorsCountToNextPart;
            _button.onClick.AddListener(UpgradeClicked);
            Draw(sprite, level, cost, value);
        }

        public void Draw(Sprite sprite, int level, int cost, float value = 0f)
        {
            _level = level;
            _image.sprite = sprite;
            _nameText.text = _configurationType.ToString();
            _levelText.text = level.ToString();
            _costText.text = NumberFormatter.FormatNumber(cost);
            _valueText.gameObject.SetActive(value != 0f);
            if (value != 0f)
            {
                _valueText.text = "x" + value.ToString("F1");
            }

            UpdateLevelIndicators();
        }

        public void ChangeInteractability(bool isActive)
        {
            _button.interactable = isActive;
        }

        private void UpgradeClicked()
        {
            OnUpgrade?.Invoke();
        }

        private void UpdateLevelIndicators()
        {
            int activeCount = (_level - 1) % _indicatorsCountToNextPart + 1;

            for (int i = 0; i < _levelIndicators.Count; i++)
            {
                _levelIndicators[i].sprite = i < activeCount ? _filledIndicatorSprite : _emptyIndicatorSprite;
            }
        }
    }
}
