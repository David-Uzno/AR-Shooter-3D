using System.Collections;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    protected float _timeLife = 1.5f;

    protected virtual void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(_timeLife);
        Destroy(gameObject);
    }

    protected abstract void HandleCollision(Collider collider);

    private void OnTriggerEnter(Collider collider)
    {
        HandleCollision(collider);
    }
}
