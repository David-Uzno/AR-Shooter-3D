using UnityEngine;

public class BulletPlayer : BulletBase
{
    [SerializeField] private float _playerTimeLife;

    protected override float GetTimeLife() => _playerTimeLife;

    protected override void HandleCollision(Collider collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(1);
                ReleaseBullet();
            }
        }
    }
}
