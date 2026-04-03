using Gameplay.Data;
using Services;
using Services.Currency;
using Services.Storage;
using Services.Time;
using UI.Screens.Variants.MainMenu;
using UnityEngine;

public class ServicesManager : MonoSingleton<ServicesManager>
{
    [SerializeField] private CurrencyCollection _currencyCollection;
    [SerializeField] private ConfigurationCollection _configurationCollection;
    
    public StorageService StorageService { get; private set; } = new();
    public CurrencyService CurrencyService { get; private set; } = new();
    public TimerService TimerService { get; private set; } = new();
    
    public ConfigurationDataService ConfigurationDataService { get; private set; } = new();

    public void Initialize()
    {
        InitializeSingleton(true);
        CurrencyService.Init(StorageService, _currencyCollection);
        ConfigurationDataService.Init(_configurationCollection);
    }
}