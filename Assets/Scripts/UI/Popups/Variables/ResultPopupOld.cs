using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Variables
{
    public class ResultPopupOld : BasePopup
    {
        [SerializeField] private TMP_Text _distanceText;
        [SerializeField] private TMP_Text _incomeMultiplierText;
        [SerializeField] private TMP_Text _earnedCoinsText;
        [SerializeField] private TMP_Text _normalButtonText;
        [SerializeField] private TMP_Text _adsButtonText;
        [SerializeField] private Button _collectNormalButton;
        [SerializeField] private Button _collectWithAdsButton;

        [Header("Wheel of Multipliers")] [SerializeField]
        private RectTransform _wheelArrow;

        [SerializeField] private float _spinDuration = 2f;
        [SerializeField] private float _spinSpeed = 360f;

        [SerializeField] private float[] _adMultipliers = { 1f, 1.5f, 2f, 1.5f, 1f };

        private float _baseIncome;
        private int _distance;
        private float _currentAdMultiplier = 2f;

        private void Start()
        {
            Show(2000, 11);
        }

        public void Show(int distanceReached, float incomeMultiplier)
        {
            gameObject.SetActive(true);

            _distance = distanceReached;
            _baseIncome = _distance * incomeMultiplier;

            _distanceText.text = $"DISTANCE REACHED : {_distance}";
            _incomeMultiplierText.text = $"INCOME : x{incomeMultiplier}";
            _earnedCoinsText.text = $"{_baseIncome:N0}";

            _normalButtonText.text = _baseIncome.ToString("N0");
            _adsButtonText.text = "????";

            _collectNormalButton.onClick.RemoveAllListeners();
            _collectWithAdsButton.onClick.RemoveAllListeners();

            _collectNormalButton.onClick.AddListener(() => CollectReward(1f));
            _collectWithAdsButton.onClick.AddListener(() => StartCoroutine(SpinWheelAndReward()));
        }

        private void CollectReward(float multiplier)
        {
            int reward = Mathf.RoundToInt(_baseIncome * multiplier);
            Debug.Log($"Collected {reward} coins!");
            gameObject.SetActive(false);
            // TODO: додати монети до профілю гравця
        }

        private IEnumerator SpinWheelAndReward()
        {
            _collectWithAdsButton.interactable = false;

            float elapsed = 0f;
            float startAngle = _wheelArrow.eulerAngles.z;
            float endAngle = startAngle + Random.Range(720, 1080); // 2–3 оберти

            while (elapsed < _spinDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _spinDuration;
                float angle = Mathf.Lerp(startAngle, endAngle, Mathf.SmoothStep(0, 1, t));
                _wheelArrow.rotation = Quaternion.Euler(0, 0, angle);
                yield return null;
            }

            float finalAngle = _wheelArrow.eulerAngles.z % 360;
            int index = GetMultiplierIndexFromAngle(finalAngle);
            _currentAdMultiplier = _adMultipliers[index];

            _adsButtonText.text = Mathf.RoundToInt(_baseIncome * _currentAdMultiplier).ToString("N0");

            Debug.Log($"Landed on x{_currentAdMultiplier} multiplier");

            yield return new WaitForSeconds(0.5f);
            CollectReward(_currentAdMultiplier);
        }

        private int GetMultiplierIndexFromAngle(float angle)
        {
            float sliceAngle = 360f / _adMultipliers.Length;
            int index = Mathf.FloorToInt(angle / sliceAngle);
            return _adMultipliers.Length - 1 - index;
        }
    }
}