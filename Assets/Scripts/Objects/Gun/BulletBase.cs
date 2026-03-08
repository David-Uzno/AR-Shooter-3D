using System.Collections;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [SerializeField] protected BulletData _bulletData;
    private float _timeLife = 1.5f;

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
        if (_bulletData != null)
        {
            return _bulletData.TimeLife;
        }
        
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

    protected virtual void HandleCollision(Collider collider)
    {
        if (_bulletData == null) return;

        int layer = collider.gameObject.layer;
        bool isCharacterLayer = layer == LayerMask.NameToLayer("Character");

        if (!isCharacterLayer) return;

        if (collider.TryGetComponent(out Character character))
        {
            character.TakeDamage(_bulletData.Damage);
            ReleaseBullet();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        HandleCollision(collider);
    }
}
