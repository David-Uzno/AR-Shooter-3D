using UnityEngine;
using UnityEngine.Android;
using TMPro;
using UnityEngine.UI;

public class TakePhotograph : MonoBehaviour
{
    [SerializeField] private Button _takePhotoButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Material _photoMaterial;

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
                // Crear una textura a partir de la imagen capturada
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("No se pudo cargar la textura desde " + path);
                    return;
                }

                // Crear una copia del material y asignar la textura al BaseMap
                if (_photoMaterial != null)
                {
                    Material newMaterial = new Material(_photoMaterial);
                    newMaterial.mainTexture = texture;
                    Debug.Log("Material creado con la textura asignada.");
                }
                else
                {
                    Debug.LogWarning("Material no asignado en el inspector.");
                }
            }
            else
            {
                UpdateStatus("Error al tomar la fotografía.");
            }
        }, maxSize);
    }
}
