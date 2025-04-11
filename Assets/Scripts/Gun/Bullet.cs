using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _shootingTimeLife = 1f;
    [SerializeField] private float _impactTimeLife = 0.25f;

    private void Start()
    {
        Destroy(gameObject, _shootingTimeLife);
    }

    /*void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
			if (collider.gameObject.GetComponent<Enemy>() != null)
			{
				collider.gameObject.GetComponent<Enemy>().ReduceLife();
			}
            Destroy(gameObject, _impactTimeLife);
        }
    }*/
}
