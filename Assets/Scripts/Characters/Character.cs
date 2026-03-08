using NaughtyAttributes;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] [Min(0)] protected int _lifeCurrent = 10;
    [SerializeField] [Min(0)] protected int _lifeMax = 10;
    public int LifeCurrent => _lifeCurrent;
    [ProgressBar("Life", "_lifeMax", color: EColor.Green)]
    private readonly int _progressBarLife;

    protected virtual void Start()
    {
        _lifeMax = Mathf.Max(1, _lifeMax);
        _lifeCurrent = _lifeMax;
    }

    public virtual void TakeDamage(int damage)
    {
        _lifeCurrent -= damage;
        if (_lifeCurrent <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    
}
