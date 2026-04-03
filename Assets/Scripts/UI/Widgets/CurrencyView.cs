using System.Linq;
using Services;
using Services.Currency;
using TMPro;
using UI.Managers;
using UI.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private CurrencyType _type;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _image;

        private CurrencyService _currencyService;

        private UIManager _uiManager;
        protected IBank bank;
        protected CurrencyCollection CurrencyCollection => _currencyService.CurrencyCollection;

        protected virtual void Start()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(ShowScreen);
            }

            _currencyService = ServicesManager.Instance.CurrencyService;
            bank = _currencyService.GetCurrencyByType(_type);
            _uiManager = UIManager.Instance;
            bank.OnCurrencyChanged += SetCurrencyText;
            SetCurrencyText(bank.Currency);
            var data = _currencyService.CurrencyCollection.CurrencySprites.FirstOrDefault(item => item.Type == _type);
            _image.sprite = data?.Value;
        }

        private void ShowScreen()
        {
            _uiManager.ScreensManager.ShowScreen(ScreenTypes.Shop);
        }

        protected virtual void OnDestroy()
        {
            _button?.onClick.RemoveListener(ShowScreen);

            if (bank != null)
            {
                bank.OnCurrencyChanged -= SetCurrencyText;
            }
        }

        private void SetCurrencyText(int value)
        {
            _text.text = NumberFormatter.FormatNumber(value);
        }
    }
}