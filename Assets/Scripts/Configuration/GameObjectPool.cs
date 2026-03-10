using System.Collections.Generic;
using UnityEngine;

public static class GameObjectPool
{
    private sealed class PoolData
    {
        public GameObject Prefab;
        public readonly Queue<GameObject> AvailableObjects = new();
        public Transform PoolRoot;
        public int TotalObjectsCount;

        public PoolData(GameObject prefab, Transform parent)
        {
            Prefab = prefab;
            PoolRoot = new GameObject($"{nameof(GameObjectPool)}_{prefab.name}").transform;
            PoolRoot.SetParent(parent);
        }
    }

    private static readonly Dictionary<int, PoolData> _poolsByPrefabId = new();
    private static readonly Dictionary<GameObject, PoolData> _poolsByObject = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        _poolsByPrefabId.Clear();
        _poolsByObject.Clear();
    }

    public static GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform caller, int initialSize = 0)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"{nameof(GameObjectPool)} no puede proporcionar objetos porque no se ha asignado un prefab.");
            return null;
        }

        PoolData pool = GetOrCreatePool(prefab, caller);
        EnsurePoolRoot(pool, caller);
        EnsurePoolCapacity(pool, initialSize);

        GameObject pooledObject = null;

        while (pool.AvailableObjects.Count > 0 && pooledObject == null)
        {
            pooledObject = pool.AvailableObjects.Dequeue();
        }

        if (pooledObject == null)
        {
            pooledObject = CreateInstance(pool, caller);
        }

        pooledObject.transform.SetParent(pool.PoolRoot, false);
        pooledObject.transform.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public static bool ReturnObject(GameObject pooledObject)
    {
        if (pooledObject == null)
        {
            return false;
        }

        if (!_poolsByObject.TryGetValue(pooledObject, out PoolData pool))
        {
            return false;
        }

        EnsurePoolRoot(pool);
        pooledObject.transform.SetParent(pool.PoolRoot, false);
        pooledObject.SetActive(false);
        pool.AvailableObjects.Enqueue(pooledObject);
        return true;
    }

    private static PoolData GetOrCreatePool(GameObject prefab, Transform parent)
    {
        int prefabId = prefab.GetInstanceID();

        if (_poolsByPrefabId.TryGetValue(prefabId, out PoolData pool))
        {
            if (pool.Prefab == null)
            {
                pool.Prefab = prefab;
            }

            return pool;
        }

        pool = new PoolData(prefab, parent);
        _poolsByPrefabId.Add(prefabId, pool);
        return pool;
    }

    private static void EnsurePoolCapacity(PoolData pool, int minimumSize)
    {
        int targetSize = Mathf.Max(0, minimumSize);

        while (pool.TotalObjectsCount < targetSize)
        {
            GameObject pooledObject = CreateInstance(pool, pool.PoolRoot);
            ReturnObject(pooledObject);
        }
    }

    private static GameObject CreateInstance(PoolData pool, Transform parent)
    {
        EnsurePoolRoot(pool, parent);

        GameObject pooledObject = Object.Instantiate(pool.Prefab, pool.PoolRoot);
        pool.TotalObjectsCount++;
        _poolsByObject[pooledObject] = pool;

        return pooledObject;
    }

    private static void EnsurePoolRoot(PoolData pool, Transform parent = null)
    {
        if (pool.PoolRoot != null)
        {
            if (parent != null && pool.PoolRoot.parent != parent)
            {
                pool.PoolRoot.SetParent(parent);
            }
            return;
        }

        pool.PoolRoot = new GameObject($"{nameof(GameObjectPool)}_{pool.Prefab.name}").transform;
        
        if (parent != null)
        {
            pool.PoolRoot.SetParent(parent);
        }
    }
}