using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gyroscope : MonoBehaviour
{
    private UnityEngine.Gyroscope _gyroscope;
    //[SerializeField] private Text _statusText;

    private void EnsureGyroscopeEnabled()
    {
        if (_gyroscope == null || !_gyroscope.enabled)
        {
            if (SystemInfo.supportsGyroscope)
            {
                _gyroscope = Input.gyro;
                _gyroscope.enabled = true;
            }
            else
            {
                Debug.LogError("El dispositivo no soporta giroscopio.");
            }
        }
    }

    void Update()
    {
        EnsureGyroscopeEnabled();

        Quaternion adjustedRotation = new Quaternion(-_gyroscope.attitude.x, -_gyroscope.attitude.y, _gyroscope.attitude.z, _gyroscope.attitude.w);
        Vector3 eulerAngles = (Quaternion.Euler(90, 0, 0) * adjustedRotation).eulerAngles;
        eulerAngles.z = 0;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
