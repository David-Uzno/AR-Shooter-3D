using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class TakePhotograph : MonoBehaviour
{
    [SerializeField] private Button _takePhotoButton;
    [SerializeField] private Material _photoMaterial;

    private int _photoCounter;

    public static event Action<string> OnPhotoTaken;

    private void Start()
    {
        // Cargar el contador desde PlayerPrefs
        _photoCounter = PlayerPrefs.GetInt("PhotoCounter", 0);

        // Solicitar permiso de cámara al iniciar
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        // Asignar eventos a los botones
        if (_takePhotoButton != null)
        {
            _takePhotoButton.onClick.AddListener(() => CapturePhoto(512));
        }
    }

    private void CapturePhoto(int maxSize)
    {
        NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                HandlePhotoTaken(path, maxSize);
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

        if (OnPhotoTaken != null)
        {
            OnPhotoTaken.Invoke(path);
        }
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
