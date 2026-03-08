using UnityEngine;

[CreateAssetMenu(fileName = "NewBulletData", menuName = "AR-Shooter/Bullet Data")]
public class BulletData : ScriptableObject
{
    [SerializeField] private float _timeLife = 1.5f;
    [SerializeField] private int _damage = 1;
    public float TimeLife => _timeLife;
    public int Damage => _damage;
}
