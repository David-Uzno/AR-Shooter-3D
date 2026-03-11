using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] private float _shotForce = 10f;
    [SerializeField] private float _shotCoolDown = 0.5f;
	private float _shotRateTime = 0;
	[SerializeField] private Transform _firePoint;

    [Header("Pool")]
    [SerializeField] private GameObject _bulletPrefab;
	[SerializeField] [Min(0)] private int _poolInitialSize = 5;

    private readonly Dictionary<int, float> _shotCooldownMultipliers = new();
    private int _nextModifierId = 1;

    public void Fire(Vector3 direction)
    {
        if (Time.time >= _shotRateTime)
        {
            GameObject newBullet = SpawnBullet();

            if (newBullet == null)
            {
                return;
            }

            if (newBullet.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.AddForce(direction * _shotForce, ForceMode.Impulse);
            }

            _shotRateTime = Time.time + GetCurrentShotCooldown();
        }
    }

    public int AddShotCooldownMultiplier(float multiplier)
    {
        if (multiplier <= 0f)
        {
            Debug.LogWarning($"{nameof(Shooting)} recibió un multiplicador inválido: {multiplier}.", this);
            return -1;
        }

        int modifierId = _nextModifierId++;
        _shotCooldownMultipliers[modifierId] = multiplier;
        return modifierId;
    }

    public bool RemoveShotCooldownMultiplier(int modifierId)
    {
        return _shotCooldownMultipliers.Remove(modifierId);
    }

    private GameObject SpawnBullet()
    {
        if (_firePoint == null)
        {
            Debug.LogWarning($"{nameof(Shooting)} en {name} no tiene punto de disparo asignado.", this);
            return null;
        }

        if (_bulletPrefab == null)
        {
            Debug.LogWarning($"{nameof(Shooting)} en {name} no tiene prefab de bala asignado.", this);
            return null;
        }

        return GameObjectPool.GetObject(_bulletPrefab, _firePoint.position, Quaternion.identity, transform, _poolInitialSize);
    }

    private float GetCurrentShotCooldown()
    {
        float currentCooldown = _shotCoolDown;

        foreach (float multiplier in _shotCooldownMultipliers.Values)
        {
            currentCooldown *= multiplier;
        }

        return Mathf.Max(0.01f, currentCooldown);
    }
}
