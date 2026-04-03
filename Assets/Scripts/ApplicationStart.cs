using System.Collections;
using Services.Currency;
using UI.Managers;
using UI.Screens;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationStart : MonoBehaviour
{
    [SerializeField] private ServicesManager _servicesManager;

    private void Awake()
    {
        _servicesManager.Initialize();
    }

    private IEnumerator Start()
    {
        EarnCurrency(100, CurrencyType.Coin);
        yield return new WaitForSeconds(0f);
        SceneManager.LoadScene("SampleScene");
    }

    private void EarnCurrency(int count, CurrencyType currencyType)
    {
        _servicesManager.CurrencyService.GetCurrencyByType(currencyType).EarnCurrency(count);
    }
}