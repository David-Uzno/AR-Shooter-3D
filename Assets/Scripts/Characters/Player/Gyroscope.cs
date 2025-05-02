using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Gyroscope : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerInput _playerInput;
    private UnityEngine.Gyroscope _gyroscope;
    private int _verificationCount = 0;
    [SerializeField] private byte _verificationAttempts = 10;
    private bool _isGyroscopeSupported = false;

    private void Start()
    {
        StartCoroutine(EnsureGyroscopeEnabledCoroutine());
    }

    private IEnumerator EnsureGyroscopeEnabledCoroutine()
    {
        while (_gyroscope == null || !_gyroscope.enabled)
        {
            Debug.Log("Verificando giroscopio.");

            if (SystemInfo.supportsGyroscope)
            {
                _gyroscope = Input.gyro;
                _gyroscope.enabled = true;
                _isGyroscopeSupported = true;
            }
            else
            {
                _verificationCount++;
                if (_verificationCount >= _verificationAttempts)
                {
                    Debug.LogError("Giroscopio no soportado.");
                    _isGyroscopeSupported = false;
                    this.enabled = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void FixedUpdate()
    {
        if (!_isGyroscopeSupported) return;

        Quaternion adjustedRotation = new Quaternion(-_gyroscope.attitude.x, -_gyroscope.attitude.y, _gyroscope.attitude.z, _gyroscope.attitude.w);
        Vector2 moveInput = (Quaternion.Euler(90, 0, 0) * adjustedRotation).eulerAngles / 360f;

        var moveAction = _playerInput.actions["Move"];
        moveAction.ApplyBindingOverride(0, new InputBinding { overridePath = moveInput.ToString() });

        Quaternion targetRotation = Quaternion.Euler(moveInput.x * 360f, moveInput.y * 360f, 0);
        _rigidbody.MoveRotation(targetRotation);
    }
}
