using UnityEngine;
using UnityEngine.UI;

public class CameraFeed : MonoBehaviour
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
    }

    void OnGUI()
    {
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            GUILayout.Label("Esperando autorización para usar la cámara...");
            return;
        }

        foreach (var divece in WebCamTexture.devices)
        {
            if (GUILayout.Button(divece.name))
            {
                SelectCamera(divece.name);
            }
        }
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
