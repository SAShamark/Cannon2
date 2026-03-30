using Gameplay.Entities.Character;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay
{
    

    public class BackgroundController : MonoBehaviour
    {
        private enum TileType { Start, Middle, End }

        [SerializeField] private TileControl _startPrefab;
        [SerializeField] private TileControl _middlePrefab;
        [SerializeField] private TileControl _endPrefab;

        [Tooltip("Total rows: 1 start + N middle + 1 end")]
        [SerializeField] private int _totalRows = 14;

        private ObjectPool _startPool;
        private ObjectPool _middlePool;
        private ObjectPool _endPool;

        private Transform _character;
        private Vector2 _tileSize;

        private readonly TileControl[,] _tiles   = new TileControl[3, 3];
        private readonly TileType[,]    _types   = new TileType[3, 3];
        private readonly Vector2Int[,]  _gridPos = new Vector2Int[3, 3];

        private Vector2Int _currentCell = Vector2Int.zero;
        private Vector3    _lastStepPos;
        public void Init(Transform characterTransform)
        {
            _character = characterTransform;
            _startPool  = new ObjectPool(_startPrefab.gameObject,  3, transform);
            _middlePool = new ObjectPool(_middlePrefab.gameObject, 6, transform);

            _tileSize    = ReadTileSize();
            _lastStepPos = _character.position;

            for (int gx = -1; gx <= 1; gx++)
            {
                for (int gy = -1; gy <= 1; gy++)
                {
                    var      worldCell = new Vector2Int(gx, gy);
                    TileType type      = TypeForCell(worldCell);

                    _tiles  [gx + 1, gy + 1] = Spawn(PoolFor(type), CellToWorld(worldCell));
                    _types  [gx + 1, gy + 1] = type;
                    _gridPos[gx + 1, gy + 1] = worldCell;
                }
            }
        }


        private void LateUpdate() => CheckCross();

        private void CheckCross()
        {
            Vector3 delta   = _character.position - _lastStepPos;
            var     stepDir = Vector2Int.zero;

            if (Mathf.Abs(delta.x) >= _tileSize.x)
            {
                stepDir.x      = (int)Mathf.Sign(delta.x);
                _lastStepPos.x += stepDir.x * _tileSize.x;
            }

            if (Mathf.Abs(delta.y) >= _tileSize.y)
            {
                stepDir.y      = (int)Mathf.Sign(delta.y);
                _lastStepPos.y += stepDir.y * _tileSize.y;
            }

            if (stepDir != Vector2Int.zero)
            {
                Step(stepDir);
            }
        }


        private void Step(Vector2Int direction)
        {
            _currentCell += direction;

            if (direction.x != 0)
            {
                RecycleColumn(direction.x);
            }
            else
            {
                RecycleRow(direction.y);
            }
        }


        private void RecycleColumn(int dx)
        {
            int staleGx = -dx, freshGx = dx;

            for (int gy = -1; gy <= 1; gy++)
            {
                int ax = staleGx + 1, ay = gy + 1;
                var newCell = new Vector2Int(_currentCell.x + freshGx, _currentCell.y + gy);
                MoveTile(ax, ay, newCell);
            }

            ShiftColumns(dx);
        }

        private void RecycleRow(int dy)
        {
            int staleGy = -dy, freshGy = dy;

            for (int gx = -1; gx <= 1; gx++)
            {
                int ax = gx + 1, ay = staleGy + 1;
                var newCell = new Vector2Int(_currentCell.x + gx, _currentCell.y + freshGy);
                MoveTile(ax, ay, newCell);
            }

            ShiftRows(dy);
        }

        private void MoveTile(int ax, int ay, Vector2Int newCell)
        {
            _gridPos[ax, ay] = newCell;
            _tiles[ax, ay].transform.position = CellToWorld(newCell);

            TileType neededType = TypeForCell(newCell);
            if (_types[ax, ay] == neededType) return;

            Vector3 pos = _tiles[ax, ay].transform.position;
            ReturnToPool(_tiles[ax, ay], _types[ax, ay]);
            _tiles[ax, ay] = Spawn(PoolFor(neededType), pos);
            _types[ax, ay] = neededType;
        }


        private void ShiftColumns(int dx)
        {
            int from = dx > 0 ? 0 : 2;
            int to   = dx > 0 ? 2 : 0;

            for (int ay = 0; ay < 3; ay++)
            {
                (TileControl t, TileType tp, Vector2Int p) tmp = (_tiles[from, ay], _types[from, ay], _gridPos[from, ay]);
                _tiles[from, ay] = _tiles[1, ay];  _types[from, ay] = _types[1, ay];  _gridPos[from, ay] = _gridPos[1, ay];
                _tiles[1, ay]    = _tiles[to, ay];  _types[1, ay]   = _types[to, ay];  _gridPos[1, ay]   = _gridPos[to, ay];
                _tiles[to, ay]   = tmp.t;            _types[to, ay]  = tmp.tp;           _gridPos[to, ay]  = tmp.p;
            }
        }

        private void ShiftRows(int dy)
        {
            int from = dy > 0 ? 0 : 2;
            int to   = dy > 0 ? 2 : 0;

            for (int ax = 0; ax < 3; ax++)
            {
                (TileControl t, TileType tp, Vector2Int p) tmp = (_tiles[ax, from], _types[ax, from], _gridPos[ax, from]);
                _tiles[ax, from] = _tiles[ax, 1];  _types[ax, from] = _types[ax, 1];  _gridPos[ax, from] = _gridPos[ax, 1];
                _tiles[ax, 1]    = _tiles[ax, to];  _types[ax, 1]   = _types[ax, to];  _gridPos[ax, 1]   = _gridPos[ax, to];
                _tiles[ax, to]   = tmp.t;            _types[ax, to]  = tmp.tp;           _gridPos[ax, to]  = tmp.p;
            }
        }
        
        private TileType TypeForCell(Vector2Int worldCell)
        {
            int row = worldCell.y;
            if (row == 0)               return TileType.Start;
            if (row == _totalRows - 1)  return TileType.End;
            return TileType.Middle;
        }


        private ObjectPool PoolFor(TileType type) => type switch
        {
            TileType.Start => _startPool,
            TileType.End   => _endPool,
            _              => _middlePool,
        };

        private TileControl Spawn(ObjectPool pool, Vector3 position)
        {
            GameObject go = pool.GetFreeElement();
            go.transform.position = position;
            return go.GetComponent<TileControl>();
        }

        private void ReturnToPool(TileControl tile, TileType type)
            => PoolFor(type).TurnOffObject(tile.gameObject);

        private Vector3 CellToWorld(Vector2Int cell) => new(cell.x * _tileSize.x, cell.y * _tileSize.y, 0f);

        private Vector2 ReadTileSize()
        {
            TileControl probe = Instantiate(_middlePrefab);
            probe.gameObject.hideFlags = HideFlags.HideAndDontSave;
            Vector2 size = probe.Size;
            DestroyImmediate(probe.gameObject);
            return size;
        }
    }
}