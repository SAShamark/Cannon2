using System.Collections.Generic;
using Gameplay.Entities.Character;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Spawn
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<SpawnEntityData> _entityData;
        [SerializeField] private Transform _container;

        [SerializeField] private float _spawnAheadDistance = 20f;
        [SerializeField] private float _spawnInterval = 10f;
        [SerializeField] private float _despawnDistance = 15f;

        private Transform _target;
        private float _nextSpawnY;
        private Dictionary<SpawnEntityData, ObjectPool> _pools;
        private readonly List<GameObject> _activeEntities = new();
        private readonly Dictionary<GameObject, ObjectPool> _entityToPool = new();

        public void Init(Transform target)
        {
            _target = target;

            _pools = new Dictionary<SpawnEntityData, ObjectPool>();

            foreach (SpawnEntityData data in _entityData)
            {
                _pools[data] = new ObjectPool(data.Entity.gameObject, data.InitialPoolSize, _container);
            }

            _nextSpawnY = _target.position.y + _spawnAheadDistance;
        }

        private void Update()
        {
            if (_target != null)
            {
                if (_target.position.y + _spawnAheadDistance >= _nextSpawnY)
                {
                    TrySpawn();
                }

                CleanUpOffscreenObjects();
            }
        }

        private void TrySpawn()
        {
            int count = Random.Range(1, 4);

            for (int i = 0; i < count; i++)
            {
                SpawnEntityData selected = GetWeightedRandom();
                if (selected == null) continue;

                ObjectPool pool = _pools[selected];
                GameObject obj = pool.GetFreeElement();

                float spawnY = _nextSpawnY + Random.Range(-3f, 3f);
                float spawnX = Random.Range(selected.MinRange, selected.MaxRange);
                obj.transform.position = new Vector3(spawnX, spawnY, 0f);

                _activeEntities.Add(obj);
                _entityToPool[obj] = pool;
            }

            _nextSpawnY += Random.Range(_spawnInterval * 0.7f, _spawnInterval * 1.3f);
        }

        private void CleanUpOffscreenObjects()
        {
            for (int i = _activeEntities.Count - 1; i >= 0; i--)
            {
                GameObject entity = _activeEntities[i];

                if (!entity.activeInHierarchy)
                {
                    _entityToPool.Remove(entity);
                    _activeEntities.RemoveAt(i);
                    continue;
                }

                if (_target.position.y - entity.transform.position.y > _despawnDistance)
                {
                    if (_entityToPool.TryGetValue(entity, out ObjectPool pool))
                    {
                        pool.TurnOffObject(entity);
                        _entityToPool.Remove(entity);
                    }

                    _activeEntities.RemoveAt(i);
                }
            }
        }

        private SpawnEntityData GetWeightedRandom()
        {
            float totalWeight = 0f;
            foreach (SpawnEntityData data in _entityData)
                totalWeight += data.SpawnChance;

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (SpawnEntityData data in _entityData)
            {
                cumulative += data.SpawnChance;
                if (roll <= cumulative)
                    return data;
            }

            return null;
        }
    }
}