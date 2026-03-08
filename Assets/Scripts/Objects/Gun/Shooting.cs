using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] private float _shotForce = 10f;
    [SerializeField] private float _shotCoolDown = 0.5f;
    private float _shotRateTime = 0;

    [Header("Dependencies")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;

    public void Fire(Vector3 direction)
    {
        if (Time.time > _shotRateTime)
        {
            // Instanciar Bala
            GameObject newBullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);

            // Fuerza de Bala
            if (newBullet.TryGetComponent<Rigidbody>(out var _rigidbody))
            {
                _rigidbody.AddForce(direction * _shotForce, ForceMode.Impulse);
            }

            // Tiempo de Cooldown
            _shotRateTime = Time.time + _shotCoolDown;
        }
    }
}
