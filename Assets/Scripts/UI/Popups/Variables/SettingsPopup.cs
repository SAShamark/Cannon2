using System;
using System.Collections.Generic;
using Audio;
using Audio.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Variables
{
    public class SettingsPopup : BasePopup
    {
        [SerializeField] private Button _panelCloseButton;
        [SerializeField] private Button _privacyPolicyButton;
        [SerializeField] private Slider _masterSlider;
        
        private AudioManager _audioManager;
        private readonly Dictionary<AudioMixerGroups, float> _initialVolumeSettings = new();
        

        private void Start()
        {
            _audioManager = AudioManager.Instance;
            Initialize();
        }
        private void Initialize()
        {
            GetInitialSettings();
            SetupUI();
            AddListeners();
        }

        private void GetInitialSettings()
        {
            _initialVolumeSettings[AudioMixerGroups.Master] = _audioManager.GetVolume(AudioMixerGroups.Master);
        }

        private void SetupUI()
        {
            _masterSlider.value = _initialVolumeSettings[AudioMixerGroups.Master];
        }

        private void AddListeners()
        {
            _panelCloseButton.onClick.AddListener(CloseTrigger);
            _privacyPolicyButton.onClick.AddListener(PrivacyPolicyButton);
           
            /*_vibrationToggle.onValueChanged.AddListener(value =>
            {
                //todo _audioManager.VibrationEnabled = value;
                ButtonClickedSound();
            });*/
            _masterSlider.onValueChanged.AddListener(value =>
                ChangeVolume(value, _masterSlider, _audioManager.ChangeMasterVolume));
        }

        private void PrivacyPolicyButton()
        {
            //todo Application.OpenURL("");
            CloseTrigger();    
        }
        public void PlaySliderSound() => _audioManager.Play(AudioGroupType.UiSounds, "Slider");

        private void ChangeVolume(float value, Slider slider, Action<float> changeVolumeAction,
            bool isSlider = true)
        {
            changeVolumeAction(value);

            if (!isSlider)
            {
                ButtonClickedSound();
                slider.value = value;
            }
        }

        public override void CloseTrigger()
        {
            SaveSettings();
            CloseButtonClickedSound();

            base.CloseTrigger();
        }

        private void SaveSettings()
        {
            _audioManager.SaveAudioSettings();
        }
    }
}