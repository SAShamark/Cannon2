using Audio;
using Audio.Data;
using UI.Managers.Components;
using UnityEngine;

namespace UI
{
    public abstract class BaseWindow : MonoBehaviour
    {
        [SerializeField]
        protected Canvas _canvas;

        [SerializeField]
        protected SafeAreaFitter _safeAreaFitter;

       

        protected void ButtonClickedSound()
        {
           // AudioManager.Instance.Play(AudioGroupType.UiSounds, "Button");
        }
        protected void CloseButtonClickedSound()
        {
            //AudioManager.Instance.Play(AudioGroupType.UiSounds, "CloseButton");
        }
    }
}