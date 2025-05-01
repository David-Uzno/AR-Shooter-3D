using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gyroscope : MonoBehaviour
{
    private UnityEngine.Gyroscope _gyroscope;

    void Start()
    {
        StartCoroutine(EnsureGyroscopeEnabledCoroutine());
    }

    private IEnumerator EnsureGyroscopeEnabledCoroutine()
    {
        while (_gyroscope == null || !_gyroscope.enabled)
        {
            Debug.Log("Verificando giroscopio");

            if (SystemInfo.supportsGyroscope)
            {
                _gyroscope = Input.gyro;
                _gyroscope.enabled = true;
            }
            else
            {
                Debug.LogError("Giroscopio no soportado.");
                yield break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    void Update()
    {
        Quaternion adjustedRotation = new Quaternion(-_gyroscope.attitude.x, -_gyroscope.attitude.y, _gyroscope.attitude.z, _gyroscope.attitude.w);
        Vector2 eulerAngles = (Quaternion.Euler(90, 0, 0) * adjustedRotation).eulerAngles;
        transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0);
    }
}
