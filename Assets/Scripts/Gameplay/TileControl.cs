using UnityEngine;

namespace Gameplay
{
    public class TileControl : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Transform _leftBound;
        [SerializeField] private Transform _rightBound;
        [SerializeField] private Transform _topBound;
        [SerializeField] private Transform _bottomBound;

        public SpriteRenderer Renderer => _renderer;
        public Transform LeftBound => _leftBound;
        public Transform RightBound => _rightBound;
        public Transform TopBound => _topBound;
        public Transform BottomBound => _bottomBound;

        public Vector2 Size => new Vector2(
            Mathf.Abs(_rightBound.position.x - _leftBound.position.x),
            Mathf.Abs(_topBound.position.y - _bottomBound.position.y)
        );
    }
}