using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed = 2;
    [SerializeField] private float _rotationSpeed = 100;

    private void Update()
    {
        Vector3 tilt = Input.acceleration;
        tilt = Quaternion.Euler(90, 0, 0) * tilt;
        _rigidbody.AddForce(tilt * _speed);

        // Rotación horizontal
        float horizontalTilt = Input.acceleration.x;
        transform.Rotate(0, horizontalTilt * _rotationSpeed * Time.deltaTime, 0);
    }
}
