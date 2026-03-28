using System;
using Services.Storage;
using Services.Time;
using TMPro;
using UnityEngine;

namespace UI.Widgets
{
    public class IdleCurrencyView : CurrencyView
    {
        [SerializeField] private TMP_Text _timerText;
        private TimerService _timerService;

        protected override void Start()
        {
            _timerService = ServicesManager.Instance.TimerService;
            _timerText.text = FormatTimeSpan(_timerService.GetRemainingTime(StorageConstants.TIMER_KEY));

            base.Start();

            bool showTimer = bank.Currency < CurrencyCollection.MaxPlaneTickets;
            _timerText.gameObject.transform.parent.gameObject.SetActive(showTimer);

            bank.OnCurrencyChanged += CurrencyChanged;
        }

        private void Update()
        {
            _timerText.text = FormatTimeSpan(_timerService.GetRemainingTime(StorageConstants.TIMER_KEY));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (bank != null)
            {
                bank.OnCurrencyChanged -= CurrencyChanged;
            }
        }

        private void CurrencyChanged(int value)
        {
            bool showTimer = value < CurrencyCollection.MaxPlaneTickets;
            _timerText.gameObject.transform.parent.gameObject.SetActive(showTimer);
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            return $"{(int)timeSpan.TotalMinutes:D2}:{timeSpan.Seconds:D2}";
        }
    }
}