using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Gallery : MonoBehaviour
{
    [SerializeField] private GameObject _imagePrefab;

    private void Awake()
    {
        string path = Application.persistentDataPath;
        int photoCounter = PlayerPrefs.GetInt("PhotoCounter", 0);

        for (int i = 1; i <= photoCounter; i++)
        {
            string photoPath = Path.Combine(path, $"SavedPhoto_{i:D4}.png");
            if (File.Exists(photoPath))
            {
                CreateImage(photoPath);
            }
        }
    }

    private void OnEnable()
    {
        TakePhotograph.OnPhotoTaken += CreateImage;
    }

    private void OnDisable()
    {
        TakePhotograph.OnPhotoTaken -= CreateImage;
    }

    private void CreateImage(string photoPath)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(File.ReadAllBytes(photoPath));

        GameObject newImage = Instantiate(_imagePrefab, transform);
        newImage.name = Path.GetFileName(photoPath);

        Image imageComponent = newImage.GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
