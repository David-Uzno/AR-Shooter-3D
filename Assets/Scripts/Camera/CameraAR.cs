using UnityEngine;
using UnityEngine.UI;

public class CameraAR : MonoBehaviour
{
    [SerializeField] private RawImage _imageCamera;
    private WebCamTexture _textureCamera;

    void Start()
    {
        if (_imageCamera == null)
        {
            Debug.LogError("Falta referencia al RawImage");
        }

        Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice frontCamera = default;
            bool hasFrontCamera = false;

            foreach (var device in WebCamTexture.devices)
            {
                if (device.isFrontFacing)
                {
                    frontCamera = device;
                    hasFrontCamera = true;
                    break;
                }
            }

            if (hasFrontCamera)
            {
                SelectCamera(frontCamera.name);
            }
            else if (WebCamTexture.devices.Length > 0)
            {
                SelectCamera(WebCamTexture.devices[0].name);
            }
            else
            {
                Debug.LogError("No se encontraron dispositivos de cámara.");
            }
        }
        else
        {
            Debug.LogError("No se otorgó autorización para usar la cámara.");
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        foreach (var device in WebCamTexture.devices)
        {
            if (GUILayout.Button(device.name))
            {
                SelectCamera(device.name);
            }
        }

        GUILayout.EndVertical();
    }

    void SelectCamera(string deviceName)
    {
        if (_textureCamera != null)
        {
            _textureCamera.Stop();
        }

        _textureCamera = new WebCamTexture(deviceName);
        _textureCamera.Play();
        _imageCamera.texture = _textureCamera;
    }
}
