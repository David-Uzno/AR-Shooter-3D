using NaughtyAttributes;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] [Min(0)] protected int _lifeCurrent = 10;
    [SerializeField] [Min(0)] protected int _lifeMax = 10;
    public int LifeCurrent => _lifeCurrent;
    public int LifeMax => _lifeMax;
    [ProgressBar("Life", "_lifeMax", color: EColor.Green)]
    private readonly int _progressBarLife;

    protected virtual void Start()
    {
        _lifeMax = Mathf.Max(1, _lifeMax);
        _lifeCurrent = _lifeMax;
        OnLifeChanged();
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        _lifeCurrent -= damage;
        _lifeCurrent = Mathf.Max(0, _lifeCurrent);
        OnLifeChanged();

        if (_lifeCurrent <= 0)
        {
            Die();
        }
    }

    public virtual int RestoreLife(int amount)
    {
        if (amount <= 0)
        {
            return 0;
        }

        int previousLife = _lifeCurrent;
        _lifeCurrent = Mathf.Clamp(_lifeCurrent + amount, 0, _lifeMax);

        if (_lifeCurrent != previousLife)
        {
            OnLifeChanged();
        }

        return _lifeCurrent - previousLife;
    }

    protected virtual void OnLifeChanged()
    {
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    
}
