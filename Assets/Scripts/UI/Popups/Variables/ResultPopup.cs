using System;
using Services.Currency;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Variables
{
    public class ResultPopup : BasePopup
    {
        [SerializeField] private TMP_Text _earnedCoinsText;
        [SerializeField] private Button _collectNormalButton;
        [SerializeField] private Button _collectWithAdsButton;

        private float _baseIncome;
        private CurrencyService _currencyService;
        public event Action OnResultClosed;

        public override void Show()
        {
            base.Show();

            _collectNormalButton.onClick.RemoveAllListeners();
            _collectWithAdsButton.onClick.RemoveAllListeners();

            _collectNormalButton.onClick.AddListener(() => CollectReward(1f));
            _collectWithAdsButton.onClick.AddListener(() => CollectAdReward(2f));
        }

        public void Draw(int distanceReached, float incomeMultiplier)
        {
            _baseIncome = distanceReached * incomeMultiplier;
            _earnedCoinsText.text = $"{_baseIncome:N0}";
        }

        private void CollectAdReward(float multiplier)
        {
            //todo: Implement ad watching logic here
            CollectReward(multiplier);
        }

        private void CollectReward(float multiplier)
        {
            int reward = Mathf.RoundToInt(_baseIncome * multiplier);
            ServicesManager.Instance.CurrencyService.GetCurrencyByType(CurrencyType.Coin).EarnCurrency(reward);
            OnResultClosed?.Invoke();
            CloseTrigger();
        }
    }
    
}