using UnityEngine;

public class Enemy : Character
{
    [Header("Stats")]
    [SerializeField] private int _points = 50;

    [Header("Shooting")]
    [SerializeField] private Shooting _shooting;
    [SerializeField] private Vector3 _targetPosition = Vector3.zero;

    private void Update()
    {
        Fire();
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
