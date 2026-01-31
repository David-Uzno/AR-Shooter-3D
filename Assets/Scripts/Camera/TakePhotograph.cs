using System;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class TakePhotograph : MonoBehaviour
{
    [SerializeField] private Button _takePhotoButton;
    [SerializeField] private Material _photoMaterial;

    private int _photoCounter;

    public static event Action<string> OnPhotoTaken;

    private string _pendingPhotoPath;
    private int _pendingMaxSize;
    private bool _hasPendingPhoto;

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
        }
    }

    private void Update()
    {
        if (!_hasPendingPhoto) return;

        _hasPendingPhoto = false;
        HandlePhotoTaken(_pendingPhotoPath, _pendingMaxSize);
    }

    private void CapturePhoto(int maxSize)
    {
        NativeCamera.TakePicture((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                _pendingPhotoPath = path;
                _pendingMaxSize = maxSize;
                _hasPendingPhoto = true;
            }
            else
            {
                Debug.LogWarning("Error al tomar la fotografía.");
            }
        }, maxSize);
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

        PhotographMetadata.SaveTexture(texture, _photoCounter);
        PhotographMetadata.SaveMetadata(texture, _photoCounter);
    }

    private Texture2D LoadTexture(string path, int maxSize)
    {
        return NativeCamera.LoadImageAtPath(path, maxSize, false, true);
    }

    private void ApplyTextureToMaterial(Texture2D texture)
    {
        if (_photoMaterial != null)
        {
            Material newMaterial = new(_photoMaterial);
            newMaterial.mainTexture = texture;
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
}
