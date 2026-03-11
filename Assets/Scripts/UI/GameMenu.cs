using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] GameObject _pausePanel;
    [SerializeField] GameObject _sceneWorld;
    [SerializeField] RawImage _imageCamera;

    private bool _isGamePaused = false;

    public void Pause()
    {
        {
            _isGamePaused = !_isGamePaused;
            PauseGame();
        }
    }

    private void PauseGame()
    {
        if (_isGamePaused)
        {
            Time.timeScale = 0;
            _pausePanel.SetActive(true);
            Application.targetFrameRate = 30;
        }
        else
        {
            Time.timeScale = 1;
            _pausePanel.SetActive(false);
            Application.targetFrameRate = 120;
        }
    }

    public void Camera()
    {
        bool sceneActive = _sceneWorld != null && _sceneWorld.activeSelf;
        bool imageActive = _imageCamera != null && _imageCamera.enabled;

        // Determinar el estado base (preferir el RawImage si existe)
        bool baseActive = _imageCamera != null ? imageActive : sceneActive;
        // Alternar el RawImage y dejar _sceneWorld siempre en el estado contrario
        bool newImageState = !baseActive;

        if (_imageCamera != null)
            _imageCamera.enabled = newImageState;
        if (_sceneWorld != null)
            _sceneWorld.SetActive(!newImageState);
    }
}
