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

    public void Fire(Vector3 direction)
    {
        if (Time.time > _shotRateTime)
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

            _shotRateTime = Time.time + _shotCoolDown;
        }
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
}
