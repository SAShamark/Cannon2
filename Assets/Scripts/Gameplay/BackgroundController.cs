using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private Sprite _startSprite;
        [SerializeField] private Sprite _middleSprite;
        [SerializeField] private Sprite _endSprite;

        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private int _poolSize = 12;
        [SerializeField] private float _tileSize  = 0f; // висота рядка (Y)
        [SerializeField] private float _tileWidth = 0f; // ширина колонки (X)

        private const int MiddleRowCount = 10;

        private Transform _characterTransform;

        private readonly Queue<GameObject> _pool = new Queue<GameObject>();
        private readonly List<TileRow> _activeRows = new List<TileRow>();

        private int _nextRowIndex = 0;

        private struct TileRow
        {
            public GameObject[] Tiles;
            public float        WorldY;
            public int          RowIndex;
        }

        // ───────────────────────────────────────────────

        public void Init(Transform characterTransform)
        {
            _characterTransform = characterTransform;

            for (int i = 0; i < _poolSize; i++)
            {
                var go = Instantiate(_tilePrefab, transform);
                go.SetActive(false);
                _pool.Enqueue(go);
            }

            // Якщо не виставлено вручну — читаємо зі спрайта
            if (_tileSize <= 0f || _tileWidth <= 0f)
                ReadTileSize();

            float charY = _characterTransform.position.y;

            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                float worldY = charY + rowOffset * _tileSize;
                SpawnRow(worldY);
            }
        }

        // ───────────────────────────────────────────────

        private void Update()
        {
            if (_characterTransform == null) return;
            CheckAndShiftRows();
        }

        private void CheckAndShiftRows()
        {
            if (_activeRows.Count == 0) return;

            TileRow topRow  = _activeRows[_activeRows.Count - 1];
            float   topEdge = topRow.WorldY + _tileSize;

            if (_characterTransform.position.y >= topEdge)
            {
                TileRow bottomRow = _activeRows[0];
                _activeRows.RemoveAt(0);
                RecycleRow(bottomRow);

                float newY = topRow.WorldY + _tileSize;
                SpawnRow(newY);
            }
        }

        // ───────────────────────────────────────────────

        private void SpawnRow(float worldY)
        {
            int    rowIndex = _nextRowIndex++;
            Sprite sprite   = GetSpriteForRow(rowIndex);

            var   tiles = new GameObject[3];
            float charX = _characterTransform != null
                ? _characterTransform.position.x
                : 0f;

            for (int col = 0; col < 3; col++)
            {
                float x = charX + (col - 1) * _tileWidth;

                var go = GetFromPool();
                go.transform.position = new Vector3(x, worldY, 0f);
                go.GetComponent<SpriteRenderer>().sprite = sprite;
                go.SetActive(true);
                tiles[col] = go;
            }

            _activeRows.Add(new TileRow
            {
                Tiles    = tiles,
                WorldY   = worldY,
                RowIndex = rowIndex
            });
        }

        private void RecycleRow(TileRow row)
        {
            foreach (var tile in row.Tiles)
                ReturnToPool(tile);
        }

        // ───────────────────────────────────────────────

        private Sprite GetSpriteForRow(int rowIndex)
        {
            if (rowIndex == 0)
                return _startSprite;

            if (rowIndex <= MiddleRowCount)
                return _middleSprite;

            return _endSprite;
        }

        private void ReadTileSize()
        {
            var go = GetFromPool();
            go.GetComponent<SpriteRenderer>().sprite = _startSprite;
            go.SetActive(true);

            var bounds = go.GetComponent<SpriteRenderer>().bounds;

            Debug.Log($"[BackgroundController] Tile bounds: x={bounds.size.x}, y={bounds.size.y}");

            if (_tileSize  <= 0f) _tileSize  = bounds.size.y;
            if (_tileWidth <= 0f) _tileWidth = bounds.size.x;

            ReturnToPool(go);
        }

        // ───────────────────────────────────────────────

        private GameObject GetFromPool()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            Debug.LogWarning("[BackgroundController] Pool exhausted, allocating extra tile.");
            return Instantiate(_tilePrefab, transform);
        }

        private void ReturnToPool(GameObject go)
        {
            go.SetActive(false);
            _pool.Enqueue(go);
        }
    }
}