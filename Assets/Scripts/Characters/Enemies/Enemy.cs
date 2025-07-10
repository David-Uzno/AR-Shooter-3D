using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] private float _life = 1f;
    [SerializeField] private int _points = 50;

    [Header("Movement")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _movementRange = 3f;

    [Header("Shooting")]
    [SerializeField] private float _shotForce = 10f;
    [SerializeField] private float _shotCoolDown = 2f;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Vector3 _targetPosition = Vector3.zero;

    private Vector3 _startPosition;
    private float _movementTimer;
    private float _shotRateTime;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        Movement();
        Fire();
    }

    private void Movement()
    {
        _movementTimer += Time.deltaTime * _speed;
        float offset = Mathf.Sin(_movementTimer) * _movementRange;
        transform.position = new Vector3(_startPosition.x + offset, _startPosition.y, _startPosition.z);
    }

    private void Fire()
    {
        if (Time.time > _shotRateTime)
        {
            GameObject newBullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
            Rigidbody _rigidbody = newBullet.GetComponent<Rigidbody>();

            if (_rigidbody != null)
            {
                Vector3 direction = (_targetPosition - _firePoint.position).normalized;
                _rigidbody.AddForce(direction * _shotForce, ForceMode.Impulse);
            }

            _shotRateTime = Time.time + _shotCoolDown;
        }
    }

    public void TakeDamage(float damage)
    {
        _life -= damage;
        if (_life <= 0)
        {
            Player player = Object.FindAnyObjectByType<Player>();
            if (player != null)
            {
                player.AddPoints(_points);
            }
            Destroy(gameObject);
        }
    }
}
