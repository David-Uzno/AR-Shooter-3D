using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPhotographs : MonoBehaviour
{
    [SerializeField] private bool _isFaceMode = true;
    [SerializeField] private float _scaleMultiplier = 3f;
    public static event Action<Image> ImageClicked;

    public void OnImageClick(Image clickedImage)
    {
        ImageClicked.Invoke(clickedImage);

        ScaleImage(clickedImage, _isFaceMode);
        CenterImage(clickedImage);
    }

    private void ScaleImage(Image image, bool _isFaceMode)
    {
        if (image.TryGetComponent<PhotographMetadata>(out PhotographMetadata originalTexture))
        {
            Texture2D texture = originalTexture.InitialTexture;
            if (_isFaceMode)
            {
                ScaleImageAsFace(image, texture);
            }
            else
            {
                ScaleImageNormally(image, texture);
            }
        }

        if (image.TryGetComponent<Button>(out Button button))
        {
            button.enabled = false;
        }
    }

    private void ScaleImageAsFace(Image image, Texture2D texture)
    {
        float aspectRatio = (float)texture.width / texture.height;
        image.transform.localScale = new Vector2(aspectRatio * _scaleMultiplier, 1 * _scaleMultiplier);

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        image.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

        if (image.TryGetComponent<Mask>(out Mask mask))
        {
            mask.showMaskGraphic = true;
        }

        if (image.TryGetComponent<Image>(out Image img))
        {
            img.type = Image.Type.Sliced;
            img.preserveAspect = true;
        }

        Material ovalMaterial = ChargeOvalMaterial();
        ApplyOvalMask(image, ovalMaterial);
    }

    private Material ChargeOvalMaterial()
    {
        Shader ovalShader = Shader.Find("Custom/OvalMaskShader");
        if (ovalShader == null)
        {
            Debug.LogError("Custom/OvalMaskShader no encontrado. Asegúrate de que el shader exista y esté configurado correctamente.");
            return null;
        }

        return new Material(ovalShader);
    }

    private void ApplyOvalMask(Image image, Material ovalMaterial)
    {
        if (ovalMaterial != null)
        {
            ovalMaterial.SetTexture("_MainTex", image.sprite.texture);
            image.material = ovalMaterial;
            Debug.Log("Material oval con shader personalizado aplicado correctamente.");
        }
    }

    private void ScaleImageNormally(Image image, Texture2D texture)
    {
        float aspectRatio = (float)texture.width / texture.height;
        image.transform.localScale = new Vector2(aspectRatio * _scaleMultiplier, 1 * _scaleMultiplier);

        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    
    private void CenterImage(Image image)
    {
        if (image.canvas.TryGetComponent<RectTransform>(out RectTransform canvasRect))
        {
            image.transform.position = canvasRect.transform.position;
        }
    }
}
