using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gyroscope : MonoBehaviour
{
    private UnityEngine.Gyroscope _gyroscope;
    //[SerializeField] private Text _statusText;

    void Start()
    {
        _gyroscope = Input.gyro;
        _gyroscope.enabled = true;
    }

    void Update()
    {
        Quaternion deviceRotation = _gyroscope.attitude;
        Quaternion adjustedRotation = new Quaternion(-deviceRotation.x, -deviceRotation.y, deviceRotation.z, deviceRotation.w);

        // Alinear rotación del dispositivo con el espacio de Unity
        Quaternion baseRotation = Quaternion.Euler(90, 45, 0);
        Quaternion worldRotation = baseRotation * adjustedRotation;

        Vector3 tilt = worldRotation.eulerAngles;
        tilt.z = 0;

        transform.rotation = Quaternion.Euler(tilt.x, tilt.y, tilt.z);

        /*if (_statusText != null)
        {
            _statusText.text = string.Format("Eje X: {0:F2}, Eje Y: {1:F2}, Eje Z: {2:F2}", tilt.x, tilt.y, tilt.z);
        }*/
    }
}
