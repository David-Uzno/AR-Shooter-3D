using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GalleryUIManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private Button _photoButton;
    [SerializeField] private GameObject _detailsUI;
    private int _currentPhotoIndex = -1;
    private GameObject _currentPhotoGameObject;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        InteractionPhotographs.ImageClicked += ShowDetailsUI;
        InteractionPhotographs.PhotoNameSelected += SetCurrentPhotoIndex;
    }

    private void OnDisable()
    {
        InteractionPhotographs.ImageClicked -= ShowDetailsUI;
        InteractionPhotographs.PhotoNameSelected -= SetCurrentPhotoIndex;
    }
    #endregion

    #region UI
    private void ShowDetailsUI(Image clickedImage)
    {
        _currentPhotoGameObject = clickedImage != null ? clickedImage.gameObject : null;
        _photoButton.gameObject.SetActive(false);
        if (_detailsUI != null)
        {
            _detailsUI.gameObject.SetActive(true);
        }
    }

    private void HideDetailsUI()
    {
        _photoButton.gameObject.SetActive(true);
        if (_detailsUI != null)
        {
            _detailsUI.gameObject.SetActive(false);
        }
        
        ShowGallery gallery = FindAnyObjectByType<ShowGallery>();
        if (gallery != null)
        {
            gallery.RestoreGalleryView();
        }
    }

    public void DeletePhoto()
    {
        if (_currentPhotoIndex <= 0)
        {
            Debug.LogWarning("Índice de foto inválido o no seleccionado.");
            return;
        }

        bool deleted = PhotoDeletionHelper.DeletePhotoAndMetadata(_currentPhotoIndex);

        // Destruir el GameObject que mostraba la foto
        if (_currentPhotoGameObject != null)
        {
            Destroy(_currentPhotoGameObject);
            _currentPhotoGameObject = null;
        }

        // Cerrar vista de detalles y resetear estado
        HideDetailsUI();

        _currentPhotoIndex = -1;

        if (deleted)
            Debug.Log("Eliminación completada.");
        else
            Debug.LogWarning("No se eliminaron archivos para la foto solicitada.");
    }
    #endregion

    #region Helpers
    private void SetCurrentPhotoIndex(string photoName)
    {
        if (int.TryParse(photoName, out int index))
        {
            _currentPhotoIndex = index;
        }
        else
        {
            Debug.LogWarning($"No se pudo convertir el nombre de foto a índice: {photoName}");
            _currentPhotoIndex = -1;
        }
    }
    #endregion
}
