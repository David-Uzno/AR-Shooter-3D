using UnityEngine;

[CreateAssetMenu(fileName = "NewBulletData", menuName = "AR-Shooter/Objects/Bullet Data")]
public class BulletData : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] private float _timeLife = 1.5f;
    [SerializeField] private int _damage = 1;

    [Header("Collision")]
    [SerializeField] private LayerMask _hitLayers = ~0;
    public LayerMask HitLayers => _hitLayers;
    public float TimeLife => _timeLife;
    public int Damage => _damage;
}
