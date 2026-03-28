using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Variables
{
    public class NoAdsPopup : BasePopup
    {
        [SerializeField] private Button _panelCloseButton;
        [SerializeField] private Button _button;

        private void Start()
        {
            _panelCloseButton.onClick.AddListener(CloseTrigger);
        }
    }
}