using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public GameObject ObjectPrefab;
    [Range(0f, 100f)] public float Probability = 0f;
}

public class GenerateObjects : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private bool _spawnOnStart = false;
    [SerializeField] private float _minSpawnInterval = 15f;
    [SerializeField] private float _maxSpawnInterval = 30f;
    [SerializeField] private float _intervalOffset = 0f;

    [Header("Objects")]
    [Range(0f, 100f)] [SerializeField] private float _spawnProbability = 100f;
    [Tooltip("0 = ilimitado. Número máximo de generaciones exitosas permitidas.")]
    [SerializeField] private byte _maxSuccessfulSpawns = 3;
    [SerializeField] private List<SpawnEntry> _spawnList = new();

    [Header("Ubication")]
    [SerializeField] private float _fixedY = 1f;
    [SerializeField] private float _maxX = 5f;
    [SerializeField] private float _minX = -5f;

    private Coroutine _spawnRoutine;
    private int _successfulSpawnCount = 0;

    private void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (_spawnRoutine == null)
        {
            _spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    public void StopSpawning()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        if (!_spawnOnStart)
        {
            float firstWait = Mathf.Max(0.01f, Random.Range(_minSpawnInterval, _maxSpawnInterval));
            yield return new WaitForSeconds(firstWait);
        }

        while (true)
        {
            // Si se alcanzó el máximo de generaciones exitosas, detener coroutine
            if (_maxSuccessfulSpawns > 0 && _successfulSpawnCount >= _maxSuccessfulSpawns)
            {
                _spawnRoutine = null;
                yield break;
            }

            // Comprobación global de probabilidad antes de intentar generar
            if (Random.value * 100f <= _spawnProbability)
            {
                SpawnOne();
            }

            float wait = Random.Range(_minSpawnInterval, _maxSpawnInterval) + _intervalOffset;
            wait = Mathf.Max(0.01f, wait);
            yield return new WaitForSeconds(wait);
        }
    }

    private void SpawnOne()
    {
        GameObject selectedPrefab = PickPrefabByProbability();
        if (selectedPrefab == null)
        {
            Debug.LogWarning("GenerateObjects: No se pudo seleccionar un prefab para spawnear.");
            return;
        }

        float spawnX = Random.Range(_minX, _maxX);
        // Usar posición local cuando el objeto instanciado se parenta a este transform
        Vector3 localPosition = new(spawnX, _fixedY, 0f);
        GameObject instance = Instantiate(selectedPrefab, transform);
        instance.transform.localPosition = localPosition;
        instance.transform.localRotation = Quaternion.identity;

        // Contar spawn exitoso y detener si alcanzó el máximo permitido
        _successfulSpawnCount++;
        if (_maxSuccessfulSpawns > 0 && _successfulSpawnCount >= _maxSuccessfulSpawns)
        {
            StopSpawning();
        }
    }

    GameObject PickPrefabByProbability()
    {
        if (_spawnList == null || _spawnList.Count == 0) return null;

        float totalProbability = 0f;
        foreach (var entry in _spawnList)
        {
            if (entry?.ObjectPrefab != null)
            {
                totalProbability += entry.Probability;
            }
        }

        // Si no hay pesos configurados, se elige uno aleatorio equiprobable
        if (totalProbability <= 0f)
        {
            var validEntries = _spawnList.FindAll(e => e?.ObjectPrefab != null);
            return validEntries.Count > 0 ? validEntries[Random.Range(0, validEntries.Count)].ObjectPrefab : null;
        }

        float randomThreshold = Random.value * totalProbability;
        float accumulatedProbability = 0f;

        foreach (var entry in _spawnList)
        {
            if (entry?.ObjectPrefab == null) continue;

            accumulatedProbability += entry.Probability;
            if (randomThreshold <= accumulatedProbability)
            {
                return entry.ObjectPrefab;
            }
        }

        return null;
    }

    void OnValidate()
    {
        if (_minSpawnInterval < 0f) _minSpawnInterval = 0f;
        if (_maxSpawnInterval < 0f) _maxSpawnInterval = 0f;
        if (_intervalOffset < 0f) _intervalOffset = 0f;

        if (_minSpawnInterval > _maxSpawnInterval)
        {
            (_maxSpawnInterval, _minSpawnInterval) = (_minSpawnInterval, _maxSpawnInterval);
        }

        if (_minX > _maxX)
        {
            (_maxX, _minX) = (_minX, _maxX);
        }
        if (_spawnList == null) _spawnList = new List<SpawnEntry>();

        // Asegurar que las probabilidades estén en 0-100
        for (int i = 0; i < _spawnList.Count; i++)
        {
            SpawnEntry entry = _spawnList[i];
            if (entry == null) continue;
            entry.Probability = Mathf.Clamp(entry.Probability, 0f, 100f);
        }

        // Asegurar que el límite máximo esté en rango 0-255
        _maxSuccessfulSpawns = (byte)Mathf.Clamp((int)_maxSuccessfulSpawns, 0, 255);
    }
}
