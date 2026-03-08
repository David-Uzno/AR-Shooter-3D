using UnityEngine;

public class Enemy : Character
{
    [Header("Stats")]
    [SerializeField] private int _points = 50;

    [Header("Movement")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _movementRange = 3f;

    [Header("Shooting")]
    [SerializeField] private Shooting _shooting;
    [SerializeField] private Vector3 _targetPosition = Vector3.zero;

    private Vector3 _startPosition;
    private float _movementTimer;

    protected override void Start()
    {
        base.Start();
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
        if (_shooting != null)
        {
            Vector3 direction = (_targetPosition - transform.position).normalized; // Usamos transform.position como base si firePoint es interno a Shooting
            // Si Shooting ya tiene su propio firePoint, la dirección es lo único que importa
            _shooting.Fire(direction);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            player.AddPoints(_points);
        }
        base.Die();
    }
}
