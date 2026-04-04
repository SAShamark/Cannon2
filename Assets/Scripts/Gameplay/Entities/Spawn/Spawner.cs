using System.Collections.Generic;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Spawn
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<SpawnEntityData> _entityData;
        [SerializeField] private Transform _container;

        [Header("Distance Settings")]
        // Дистанція від гравця, де з'являються об'єкти
        [SerializeField] private float _baseSpawnDistance = 20f; 
        // Дистанція деспавну (має бути значно більшою за спавн)
        [SerializeField] private float _despawnDistance = 40f; 

        [Header("Spawn Rate")]
        [SerializeField] private float _spawnInterval = 1.5f;

        private Transform _target;
        private float _spawnTimer;
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
        }

        private void Update()
        {
            if (_target == null) return;

            // Логіка таймера
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= _spawnInterval)
            {
                TrySpawn();
                _spawnTimer = 0;
            }

            CleanUpOffscreenObjects();
        }

        private void TrySpawn()
        {
            // Визначаємо напрямок руху. 
            // Якщо є Rigidbody2D, беремо velocity. Якщо ні — беремо напрямок носа (_target.up)
            Vector2 moveDir = _target.up; 
            var rb = _target.GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.magnitude > 0.1f)
            {
                moveDir = rb.linearVelocity.normalized;
            }

            int count = Random.Range(1, 4);

            for (int i = 0; i < count; i++)
            {
                SpawnEntityData selected = GetWeightedRandom();
                if (selected == null) continue;

                ObjectPool pool = _pools[selected];
                GameObject obj = pool.GetFreeElement();

                // Головний секрет Into Space: спавн у конусі попереду гравця
                // Додаємо випадкове відхилення від курсу (наприклад, +/- 70 градусів)
                float randomAngle = Random.Range(-70f, 70f);
                Vector3 spawnDir = Quaternion.Euler(0, 0, randomAngle) * moveDir;

                // Розраховуємо позицію: позиція гравця + напрямок * дистанцію
                // Додаємо невеликий рандом до дистанції, щоб не було ідеального кола
                float dist = _baseSpawnDistance + Random.Range(-5f, 5f);
                Vector3 spawnPos = _target.position + (spawnDir * dist);
                spawnPos.z = 0;

                obj.transform.position = spawnPos;

                _activeEntities.Add(obj);
                _entityToPool[obj] = pool;
            }
        }

        private void CleanUpOffscreenObjects()
        {
            for (int i = _activeEntities.Count - 1; i >= 0; i--)
            {
                GameObject entity = _activeEntities[i];

                if (entity == null || !entity.activeInHierarchy)
                {
                    _entityToPool.Remove(entity);
                    _activeEntities.RemoveAt(i);
                    continue;
                }

                float currentDist = Vector3.Distance(_target.position, entity.transform.position);
                
                if (currentDist > _despawnDistance)
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