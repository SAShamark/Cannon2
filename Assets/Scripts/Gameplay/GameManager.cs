using Gameplay.Character;
using UI.Screens.Variants;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CharacterControl _characterControl;
        [SerializeField] private GameplayScreen _gameplayScreen;
        [SerializeField] private BackgroundController _backgroundController;

        private void Start()
        {
            _backgroundController.Init(_characterControl.transform);
            _gameplayScreen.Init();
            _gameplayScreen.OnLaunched += CharacterLaunched;
        }

        private void CharacterLaunched(float fuelValue)
        {
            _characterControl.Initialize(fuelValue);
        }
    }
}