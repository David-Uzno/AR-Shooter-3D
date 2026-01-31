using UnityEngine;
using UnityEngine.InputSystem;

public class Gyroscope : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerInput _playerInput;
    private bool _isGyroscopeSupported = false;

    private void Awake()
    {
        if (SystemInfo.supportsGyroscope)
        {
            _isGyroscopeSupported = true;
        }
        else
        {
            Debug.LogError("Giroscopio no es compatible con este dispositivo.");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (_isGyroscopeSupported && AttitudeSensor.current != null)
        {
            if (!AttitudeSensor.current.enabled)
            {
                Debug.Log("Sensor de actitud no habilitado. Habilitando ahora.");
                InputSystem.EnableDevice(AttitudeSensor.current);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isGyroscopeSupported || AttitudeSensor.current == null || !AttitudeSensor.current.enabled)
            return;

        // Leer los datos de attitudesensor
        Quaternion gyroAttitude = AttitudeSensor.current.attitude.ReadValue();
        Quaternion adjustedRotation = new Quaternion(-gyroAttitude.x, -gyroAttitude.y, gyroAttitude.z, gyroAttitude.w);
        Vector2 moveInput = (Quaternion.Euler(90, 0, 0) * adjustedRotation).eulerAngles / 360f;

        // Aplicar input binding override
        var moveAction = _playerInput.actions["Move"];
        moveAction.ApplyBindingOverride(0, new InputBinding { overridePath = moveInput.ToString() });

        // Aplicar rotación al rigidbody
        Quaternion targetRotation = Quaternion.Euler(moveInput.x * 360f, moveInput.y * 360f, 0);
        _rigidbody.MoveRotation(targetRotation);
    }
}
