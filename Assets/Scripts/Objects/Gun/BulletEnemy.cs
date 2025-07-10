using UnityEngine;

public class BulletEnemy : BulletBase
{
    [SerializeField] private float _enemyTimeLife;

    protected override void Start()
    {
        _timeLife = _enemyTimeLife;
        base.Start();
    }

    protected override void HandleCollision(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(1);
                Destroy(gameObject);
            }
        }
    }
}
