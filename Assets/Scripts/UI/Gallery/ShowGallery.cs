using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ShowGallery : MonoBehaviour
{
    [SerializeField] private GameObject _imagePrefab;

    private void Awake()
    {
        string path = FilePaths.SavedPhotosPath;
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
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(File.ReadAllBytes(photoPath));

        GameObject newImage = Instantiate(_imagePrefab, transform);
        newImage.name = Path.GetFileName(photoPath);

        OriginalTexture originalTextureComponent = newImage.AddComponent<OriginalTexture>();
        originalTextureComponent.Texture = texture;

        if (newImage.TryGetComponent<Image>(out Image imageComponent))
        {
            int size = Mathf.Min(texture.width, texture.height);
            Rect cropRect = new Rect((texture.width - size) / 2, (texture.height - size) / 2, size, size);
            Sprite sprite = Sprite.Create(texture, cropRect, new Vector2(0.5f, 0.5f));
            imageComponent.sprite = sprite;

            RectTransform rectTransform = newImage.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void DisableImages(Image clickedImage)
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Image>(out Image imageComponent))
            {
                imageComponent.enabled = imageComponent == clickedImage;
            }
        }

        if (TryGetComponent<GridLayoutGroup>(out GridLayoutGroup gridLayoutGroup))
        {
            gridLayoutGroup.enabled = false;
        }
    }
}
