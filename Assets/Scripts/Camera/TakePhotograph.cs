using UnityEngine;
using UnityEngine.Android;
using TMPro;
using UnityEngine.UI;

public class TakePhotograph : MonoBehaviour
{
    [SerializeField] private Button _takePhotoButton;
    [SerializeField] private Button _recordVideoButton;
    [SerializeField] private TextMeshProUGUI _statusText;

    private void Start()
    {
        // Solicitar permiso de cámara al iniciar
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            UpdateStatus("Solicitando permiso de cámara...");
        }
        else
        {
            UpdateStatus("Permiso de cámara concedido.");
        }

        // Asignar eventos a los botones
        if (_takePhotoButton != null)
        {
            _takePhotoButton.onClick.AddListener(() => TakePicture(512));
        }

        if (_recordVideoButton != null)
        {
            _recordVideoButton.onClick.AddListener(RecordVideo);
        }
    }

    private void UpdateStatus(string message)
    {
        if (_statusText != null)
        {
            _statusText.text = message;
        }
        Debug.Log(message);
    }

    private void TakePicture(int maxSize)
    {
        UpdateStatus("Tomando fotografía...");
        NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                UpdateStatus("Foto guardada en: " + path);
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                Material material = quad.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Standard");

                material.mainTexture = texture;

                Destroy(quad, 5f);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                Destroy(texture, 5f);
            }
            else
            {
                UpdateStatus("Error al tomar la fotografía.");
            }
        }, maxSize);
    }

    private void RecordVideo()
    {
        UpdateStatus("Grabando video...");
        NativeCamera.RecordVideo((path) =>
        {
            if (path != null)
            {
                UpdateStatus("Video guardado en: " + path);
                // Play the recorded video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
            else
            {
                UpdateStatus("Error al grabar el video.");
            }
        });
    }
}
