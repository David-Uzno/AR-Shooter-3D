using UnityEngine;
using UnityEngine.UI;

public class GalleryUIManager : MonoBehaviour
{
    [SerializeField] private Button _photoButton;
    [SerializeField] private GameObject _detailsUI;

    private void OnEnable()
    {
        InteractionPhotographs.ImageClicked += ShowDetailsUI;
    }

    private void OnDisable()
    {
        InteractionPhotographs.ImageClicked -= ShowDetailsUI;
    }

    private void ShowDetailsUI(Image clickedImage)
    {
        _photoButton.gameObject.SetActive(false);
        if (_detailsUI != null)
        {
            _detailsUI.gameObject.SetActive(true);
        }
    }
}
