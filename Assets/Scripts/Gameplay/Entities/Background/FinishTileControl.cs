using UnityEngine;

namespace Gameplay.Entities.Background
{
    public class FinishTileControl : TileControl
    {
        [SerializeField] private FinishHandler _finishHandler;

        public FinishHandler FinishHandler => _finishHandler;
    }
}