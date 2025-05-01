using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] GameObject _pausePanel;
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
}
