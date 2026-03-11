using UnityEngine;
using System.Collections.Generic;

public class Hordes : MonoBehaviour
{
    [System.Serializable]
    private class SpawnableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField, Range(0f, 100f)] private float _spawnProbability;

        public GameObject Prefab => _prefab;
        public float SpawnProbability => _spawnProbability;
    }

    [SerializeField] private List<SpawnableObject> _spawnableObjects = new();
    [SerializeField] private Vector3 _spawnPosition = new(0, 0, 5);
    [SerializeField] private int _maxTotalSpawns = 10;

    private GameObject _currentSpawnedInstance;
    private int _spawnCount = 0;

    private void Start()
    {
        TrySpawnNext();
    }

    private void Update()
    {
        // El sistema debe detectar cuando el objeto instanciado es destruido
        if (_currentSpawnedInstance == null && _spawnCount < _maxTotalSpawns)
        {
            TrySpawnNext();
        }
    }

    private void TrySpawnNext()
    {
        if (_spawnCount >= _maxTotalSpawns) return;

        GameObject prefabToSpawn = GetRandomPrefabByProbability();

        if (prefabToSpawn != null)
        {
            _currentSpawnedInstance = Instantiate(prefabToSpawn, _spawnPosition, Quaternion.identity);
            _spawnCount++;
        }
    }

    private GameObject GetRandomPrefabByProbability()
    {
        float totalWeight = 0f;
        foreach (var obj in _spawnableObjects)
        {
            totalWeight += obj.SpawnProbability;
        }

        if (totalWeight <= 0f) return null;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var obj in _spawnableObjects)
        {
            cumulativeWeight += obj.SpawnProbability;
            if (randomValue <= cumulativeWeight)
            {
                return obj.Prefab;
            }
        }

        return null;
    }
}
