using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _life = 1f;

    public void TakeDamage(float damage)
    {
        _life -= damage;
        if (_life <= 0)
        {
           Destroy(gameObject);
        }
    }
}
