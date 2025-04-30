using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] private float _shotForce = 10f;
    [SerializeField] private float _shotCoolDown = 0.5f;
    private float _shotRateTime = 0;

	[Header("Dependencies")]
	[SerializeField] private PlayerInput _playerInput;
	[SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;

    private void Update()
    {
        Fire();
    }

	private void Fire()
	{
		if (_playerInput.actions["Attack"].ReadValue<float>() > 0)
		{
			if (Time.time > _shotRateTime)
			{
				// Instanciar Bala
				GameObject newBullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
				Rigidbody _rigidbody = newBullet.GetComponent<Rigidbody>();

				// Fuerza de Bala
				if (_rigidbody != null)
				{
					_rigidbody.AddForce(transform.forward * _shotForce, ForceMode.Impulse);
				}

				// Tiempo de Cooldown
				_shotRateTime = Time.time + _shotCoolDown;
			}
		}
	}
}
