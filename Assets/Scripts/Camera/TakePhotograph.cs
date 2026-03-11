using System;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class TakePhotograph : MonoBehaviour
{
    #region Variables
    private int _photoCounter;
    [SerializeField] private int _maxPhotos = 14;

    [Header("Dependencies")]
    [SerializeField] private Button _takePhotoButton;
    [SerializeField] private Material _photoMaterial;

    public static event Action<string> OnPhotoTaken;

    private string _pendingPhotoPath;
    private int _pendingMaxDimension;
    private bool _hasPendingPhoto;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _photoCounter = PlayerPrefs.GetInt("PhotoCounter", 0);

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        if (_takePhotoButton != null)
        {
            _takePhotoButton.onClick.AddListener(() => CapturePhoto(512));
            if (_photoCounter >= _maxPhotos)
            {
                _takePhotoButton.interactable = false;
                Debug.Log("Límite de fotografías alcanzado. No se permiten más capturas.");
            }
        }
    }

    private void Update()
    {
        if (!_hasPendingPhoto) return;

        _hasPendingPhoto = false;
        HandlePhotoTaken(_pendingPhotoPath, _pendingMaxDimension);
    }
    #endregion

    #region Photo Logic
    private void CapturePhoto(int maxDimension)
    {
        if (_photoCounter >= _maxPhotos)
        {
            Debug.LogWarning("No se puede capturar: se alcanzó el número máximo de fotografías permitidas.");
            return;
        }
        NativeCamera.TakePicture((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                _pendingPhotoPath = path;
                _pendingMaxDimension = maxDimension;
                _hasPendingPhoto = true;
            }
            else
            {
                Debug.LogWarning("Error al tomar la fotografía.");
            }
        }, maxDimension);
    }

    private void HandlePhotoTaken(string path, int maxSize)
    {
        Texture2D texture = LoadTexture(path, maxSize);
        if (texture == null)
        {
            Debug.Log("No se pudo cargar la textura desde " + path);
            return;
        }

        PhotoSaving(texture);
        ApplyTextureToMaterial(texture);

        OnPhotoTaken?.Invoke(GetSavedPhotoPath(_photoCounter));
    }

    private void PhotoSaving(Texture2D texture)
    {
        _photoCounter++;
        PlayerPrefs.SetInt("PhotoCounter", _photoCounter);
        PlayerPrefs.Save();

        EnsurePhotoDirectoryExists();

        PhotographMetadata.SaveTexture(texture, _photoCounter);
        PhotographMetadata.SaveMetadata(texture, _photoCounter);

        if (_photoCounter >= _maxPhotos)
        {
            if (_takePhotoButton != null)
            {
                _takePhotoButton.interactable = false;
            }
            Debug.Log("Se alcanzó el número máximo de fotografías permitidas. Captura deshabilitada.");
        }
    }

    private void EnsurePhotoDirectoryExists()
    {
        string path = FilePaths.SavedPhotographsPath;
        string directory = path;
        string fileName = Path.GetFileName(path);
        if (Path.HasExtension(path) || (!string.IsNullOrEmpty(fileName) && fileName.StartsWith("SavedPhoto_", StringComparison.OrdinalIgnoreCase)))
        {
            string directoryPath = Path.GetDirectoryName(path);
            if (directoryPath != null)
            {
                directory = directoryPath;
            }
            else
            {
                directory = path;
            }
        }

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
    #endregion

    #region Helpers
    private Texture2D LoadTexture(string path, int maxSize)
    {
        return NativeCamera.LoadImageAtPath(path, maxSize, false, true);
    }

    private void ApplyTextureToMaterial(Texture2D texture)
    {
        if (_photoMaterial != null)
        {
            Material newMaterial = new(_photoMaterial)
            {
                mainTexture = texture
            };
            Debug.Log("Material creado con la textura asignada.");
        }
        else
        {
            Debug.LogWarning("Material no asignado en el inspector.");
        }
    }

    private string GetSavedPhotoPath(int index)
    {
        string directory = FilePaths.SavedPhotographsPath;
        string fileName = $"SavedPhoto_{index:D4}.png";
        return Path.Combine(directory, fileName);
    }
    #endregion
}
