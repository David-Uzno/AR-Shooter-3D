using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPhotographs : MonoBehaviour
{
    [SerializeField] private float _scaleMultiplier = 3f;
    [SerializeField] private GameObject _photographsPanel;
    public static event Action<Image> ImageClicked;

    private void Start()
    {
        string savePath = FilePaths.SavedPhotosPath;
    }

    public void OnImageClick(Image clickedImage)
    {
        ImageClicked.Invoke(clickedImage);

        ScaleImage(clickedImage);
        CenterImage(clickedImage);
    }

    public void ScaleImage(Image image)
    {
        if (image.TryGetComponent<OriginalTexture>(out OriginalTexture originalTexture))
        {
            Texture2D texture = originalTexture.Texture;
            float aspectRatio = (float)texture.width / texture.height;
            image.transform.localScale = new Vector2(aspectRatio * _scaleMultiplier, 1 * _scaleMultiplier);

            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        if (image.TryGetComponent<Button>(out Button button))
        {
            button.enabled = false;
        }
    }

    public void CenterImage(Image image)
    {
        if (image.canvas.TryGetComponent<RectTransform>(out RectTransform canvasRect))
        {
            image.transform.position = canvasRect.transform.position;
        }
    }
}
