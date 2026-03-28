using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Variables.Loading
{
    public class LoadingPopup : BasePopup
    {
        [SerializeField]
        private Image _progressImage;

        public void Init()
        {
            UpdateProgress(0);
        }

        public void UpdateProgress(float progress)
        {
            _progressImage.DOKill();
            _progressImage.DOFillAmount(progress, 0.5f).SetEase(Ease.InOutQuad);
        }
    }
}