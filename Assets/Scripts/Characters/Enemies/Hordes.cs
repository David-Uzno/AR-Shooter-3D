using UnityEngine;
using UnityEngine.SceneManagement;
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
    private bool _hasLoadedWinner = false;

    private void Start()
    {
        TrySpawnNext();
    }

    private void Update()
    {
        // El sistema debe detectar cuando el objeto instanciado es destruido
        if (_currentSpawnedInstance == null)
        {
            if (_spawnCount < _maxTotalSpawns)
            {
                TrySpawnNext();
            }
            else
            {
                if (!_hasLoadedWinner)
                {
                    _hasLoadedWinner = true;
                    SceneManager.LoadScene("Winner");
                }
            }
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
        foreach (var spawnableObject in _spawnableObjects)
        {
            totalWeight += spawnableObject.SpawnProbability;
        }

        if (totalWeight <= 0f) return null;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var spawnableObject in _spawnableObjects)
        {
            cumulativeWeight += spawnableObject.SpawnProbability;
            if (randomValue <= cumulativeWeight)
            {
                return spawnableObject.Prefab;
            }
        }

        return null;
    }
}
