using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShowGallery : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject _imagePrefab;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        int photoCounter = PlayerPrefs.GetInt("PhotoCounter", 0);

        for (int i = 1; i <= photoCounter; i++)
        {
            CreateImage(GetSavedPhotoPath(i));
        }
    }

    private void OnEnable()
    {
        TakePhotograph.OnPhotoTaken += CreateImage;
        InteractionPhotographs.ImageClicked += DisableImages;
    }

    private void OnDisable()
    {
        TakePhotograph.OnPhotoTaken -= CreateImage;
        InteractionPhotographs.ImageClicked -= DisableImages;
    }
    #endregion

    #region Gallery Logic
    private void CreateImage(string photoPath)
    {
        if (_imagePrefab == null)
        {
            Debug.LogWarning("Prefab de imagen no asignado.");
            return;
        }

        string resolvedPath = ResolvePhotoPath(photoPath);
        if (string.IsNullOrEmpty(resolvedPath))
        {
            Debug.LogWarning($"Archivo no encontrado: {photoPath}");
            return;
        }

        try
        {
            Texture2D texture = new(2, 2);
            bool loaded = texture.LoadImage(File.ReadAllBytes(resolvedPath));
            if (!loaded)
            {
                Debug.LogWarning($"LoadImage falló: {resolvedPath}");
                return;
            }

            GameObject newImage = Instantiate(_imagePrefab, transform);
            newImage.name = Path.GetFileName(resolvedPath);

            PhotographMetadata originalTextureComponent = newImage.AddComponent<PhotographMetadata>();
            originalTextureComponent.InitialTexture = texture;

            if (newImage.TryGetComponent(out Image imageComponent))
            {
                int size = Mathf.Min(texture.width, texture.height);
                Rect cropRect = new((texture.width - size) / 2, (texture.height - size) / 2, size, size);
                Sprite sprite = Sprite.Create(texture, cropRect, new Vector2(0.5f, 0.5f));
                imageComponent.sprite = sprite;

                // Asegura que el shader/material correcto se aplique en todas las plataformas
                // Reemplaza "YourCustomMaterial" por el material que usas en el Editor
#if UNITY_ANDROID
                Material galleryMaterial = Resources.Load<Material>("OvalMaskShader");
                if (galleryMaterial != null)
                {
                    imageComponent.material = galleryMaterial;
                }
#endif

                if (newImage.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    rectTransform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }

    private void DisableImages(Image clickedImage)
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Image imageComponent))
            {
                imageComponent.enabled = imageComponent == clickedImage;
            }
        }

        if (TryGetComponent(out GridLayoutGroup gridLayoutGroup))
        {
            gridLayoutGroup.enabled = false;
        }
    }

    // Public helper to restore gallery thumbnails and layout after closing details
    public void RestoreGalleryView()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Image imageComponent))
            {
                imageComponent.enabled = true;
            }
        }

        if (TryGetComponent(out GridLayoutGroup gridLayoutGroup))
        {
            gridLayoutGroup.enabled = true;
        }
    }
    #endregion

    #region Helpers
    private string GetSavedPhotoPath(int index)
    {
        string directory = FilePaths.SavedPhotographsPath;
        string fileName = $"SavedPhoto_{index:D4}.png";
        return Path.Combine(directory, fileName);
    }

    private string ResolvePhotoPath(string photoPath)
    {
        if (File.Exists(photoPath))
        {
            return photoPath;
        }

        string fileName = Path.GetFileName(photoPath);
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        string originalDirectory = Path.GetDirectoryName(photoPath);
        string parentDirectory = !string.IsNullOrEmpty(originalDirectory)
            ? Path.GetDirectoryName(originalDirectory)
            : null;

        string savedDirectory = FilePaths.SavedPhotographsPath;
        string savedParentDirectory = !string.IsNullOrEmpty(savedDirectory)
            ? Path.GetDirectoryName(savedDirectory)
            : null;

        string savedChildDirectory = !string.IsNullOrEmpty(originalDirectory)
            ? Path.Combine(originalDirectory, "SavedPhoto_")
            : null;

        foreach (string directory in new[] { parentDirectory, savedDirectory, savedParentDirectory, savedChildDirectory })
        {
            if (string.IsNullOrEmpty(directory))
            {
                continue;
            }

            string candidatePath = Path.Combine(directory, fileName);
            if (File.Exists(candidatePath))
            {
                return candidatePath;
            }
        }

        return null;
    }
    #endregion
}
