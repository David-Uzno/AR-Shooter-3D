using UnityEngine;

public class BulletEnemy : BulletBase
{
    [SerializeField] private float _enemyTimeLife;

    protected override float GetTimeLife() => _enemyTimeLife;

    protected override void HandleCollision(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.TryGetComponent<Player>(out var player))
            {
                player.TakeDamage(1);
                ReleaseBullet();
            }
        }
    }
}
