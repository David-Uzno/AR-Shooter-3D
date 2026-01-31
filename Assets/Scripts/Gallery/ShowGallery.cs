using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShowGallery : MonoBehaviour
{
    [SerializeField] private GameObject _imagePrefab;

    private void Awake()
    {
        string path = FilePaths.SavedPhotographsPath;
        int photoCounter = PlayerPrefs.GetInt("PhotoCounter", 0);

        for (int i = 1; i <= photoCounter; i++)
        {
            string photoPath = path + $"{i:D4}.png";

            if (File.Exists(photoPath))
            {
                CreateImage(photoPath);
            }
            else
            {
                Debug.LogWarning("Archivo no encontrado: " + photoPath);
            }
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

    private void CreateImage(string photoPath)
    {
        if (_imagePrefab == null)
        {
            Debug.LogWarning("Prefab de imagen no asignado.");
            return;
        }

        if (!File.Exists(photoPath))
        {
            Debug.LogWarning($"Archivo no encontrado: {photoPath}");
            return;
        }

        try
        {
            Texture2D texture = new(2, 2);
            bool loaded = texture.LoadImage(File.ReadAllBytes(photoPath));
            if (!loaded)
            {
                Debug.LogWarning($"LoadImage falló: {photoPath}");
                return;
            }

            GameObject newImage = Instantiate(_imagePrefab, transform);
            newImage.name = Path.GetFileName(photoPath);

            PhotographMetadata originalTextureComponent = newImage.AddComponent<PhotographMetadata>();
            originalTextureComponent.InitialTexture = texture;

            if (newImage.TryGetComponent(out Image imageComponent))
            {
                int size = Mathf.Min(texture.width, texture.height);
                Rect cropRect = new((texture.width - size) / 2, (texture.height - size) / 2, size, size);
                Sprite sprite = Sprite.Create(texture, cropRect, new Vector2(0.5f, 0.5f));
                imageComponent.sprite = sprite;

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
}
