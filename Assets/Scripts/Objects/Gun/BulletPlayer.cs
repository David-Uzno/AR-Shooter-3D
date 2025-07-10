using UnityEngine;

public class BulletPlayer : BulletBase
{
    [SerializeField] private float _playerTimeLife;

    protected override void Start()
    {
        _timeLife = _playerTimeLife;
        base.Start();
    }

    protected override void HandleCollision(Collider collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
                Destroy(gameObject);
            }
        }
    }
}
