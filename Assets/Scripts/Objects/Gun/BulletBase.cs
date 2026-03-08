using System.Collections;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    protected float _timeLife = 1.5f;

    private Coroutine _destroyCoroutine;
    private Rigidbody _rigidbody;

    protected virtual void Awake()
    {
        TryGetComponent(out _rigidbody);
    }

    private void OnEnable()
    {
        ResetPhysics();

        if (_destroyCoroutine != null)
        {
            StopCoroutine(_destroyCoroutine);
        }

        _destroyCoroutine = StartCoroutine(DestroyBullet());
    }

    private void OnDisable()
    {
        if (_destroyCoroutine != null)
        {
            StopCoroutine(_destroyCoroutine);
            _destroyCoroutine = null;
        }

        ResetPhysics();
    }

    protected void ReleaseBullet()
    {
        if (GameObjectPool.ReturnObject(gameObject))
        {
            return;
        }

        Destroy(gameObject);
    }

    protected virtual float GetTimeLife()
    {
        return _timeLife;
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(GetTimeLife());
        _destroyCoroutine = null;
        ReleaseBullet();
    }

    private void ResetPhysics()
    {
        if (_rigidbody == null)
        {
            return;
        }

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    protected abstract void HandleCollision(Collider collider);

    private void OnTriggerEnter(Collider collider)
    {
        HandleCollision(collider);
    }
}
