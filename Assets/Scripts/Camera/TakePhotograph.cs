using UnityEngine;
using UnityEngine.Android;
using TMPro;
using UnityEngine.UI;

public class TakePhotograph : MonoBehaviour
{
    [SerializeField] private Button _takePhotoButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Material _photoMaterial;

    private int _photoCounter;

    private void Start()
    {
        // Cargar el contador desde PlayerPrefs
        _photoCounter = PlayerPrefs.GetInt("PhotoCounter", 0);

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
                HandlePhotoTaken(path, maxSize);
            }
            else
            {
                UpdateStatus("Error al tomar la fotografía.");
            }
        }, maxSize);
    }

    private void HandlePhotoTaken(string path, int maxSize)
    {
        UpdateStatus("Foto guardada en: " + path);
        Texture2D texture = LoadTexture(path, maxSize);
        if (texture == null)
        {
            Debug.Log("No se pudo cargar la textura desde " + path);
            return;
        }

        SaveTexture(texture);
        ApplyTextureToMaterial(texture);
    }

    private Texture2D LoadTexture(string path, int maxSize)
    {
        return NativeCamera.LoadImageAtPath(path, maxSize, false, true);
    }

    private void SaveTexture(Texture2D texture)
    {
        _photoCounter++;
        PlayerPrefs.SetInt("PhotoCounter", _photoCounter);
        PlayerPrefs.Save();

        string savePath = Application.persistentDataPath + $"/SavedPhoto_{_photoCounter:D4}.png";
        System.IO.File.WriteAllBytes(savePath, texture.EncodeToPNG());
        Debug.Log("Textura guardada en: " + savePath);
    }

    private void ApplyTextureToMaterial(Texture2D texture)
    {
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
}
