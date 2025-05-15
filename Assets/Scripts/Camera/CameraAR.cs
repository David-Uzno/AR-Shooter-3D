using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraAR : MonoBehaviour
{
    [SerializeField] private RawImage _imageCamera;
    private WebCamTexture _textureCamera;
    private WebCamDevice[] _devices;
    private bool _waitingForPermission;

    private IEnumerator Start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam)) yield return InitializeCamera();
        else _waitingForPermission = true;
    }

    private void Update()
    {
        if (_waitingForPermission && Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            _waitingForPermission = false;
            StartCoroutine(InitializeCamera());
        }
    }

    private IEnumerator InitializeCamera()
    {
        while ((_devices = WebCamTexture.devices).Length == 0)
        {
            Debug.Log("No se encontraron dispositivos de cámara. Reintentando...");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Dispositivos de cámara encontrados.");
        SelectCamera(System.Array.Find(_devices, d => !d.isFrontFacing).name);
    }

    private void OnGUI()
    {
        if (_devices == null) return;
        foreach (var device in _devices)
            if (GUILayout.Button(device.name)) SelectCamera(device.name);
    }

    private void SelectCamera(string deviceName)
    {
        if (_textureCamera != null)
        {
            _textureCamera.Stop();
        }

        _textureCamera = new WebCamTexture(deviceName);
        _imageCamera.texture = _textureCamera;
        _textureCamera.Play();

        if (!_textureCamera.isPlaying)
        {
            Debug.Log("No se pudo iniciar la cámara: " + deviceName);
            _imageCamera.texture = null;
            _textureCamera = null;
        }
    }

    private void OnDisable()
    {
        if (_textureCamera != null)
        {
            _textureCamera.Stop();
        }
        _textureCamera = null;
        _waitingForPermission = false;
    }
}
