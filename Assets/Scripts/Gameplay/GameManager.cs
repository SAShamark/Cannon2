using Gameplay.Entities.Background;
using Gameplay.Entities.Character;
using UI;
using UI.Managers;
using UI.Screens;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BackgroundController _backgroundController;
        [SerializeField] private CharacterControl _characterControl;
        [SerializeField] private GameplayManager _gameplayManager;

        private MainMenuScreen _mainMenuScreen;

        private void Start()
        {
            _backgroundController.Init(_characterControl.transform);
            
            UIManager.Instance.ScreensManager.ShowScreen(ScreenTypes.MainMenu);
            _mainMenuScreen = UIManager.Instance.ScreensManager.GetScreen(ScreenTypes.MainMenu) as MainMenuScreen;
            _mainMenuScreen.OnStartGameplay += StartGameplay;
        }

        private void OnDestroy()
        {
            _mainMenuScreen.OnStartGameplay -= StartGameplay;
        }

        private void StartGameplay()
        {
            _gameplayManager.Init(_backgroundController, _characterControl);
        }
    }
}