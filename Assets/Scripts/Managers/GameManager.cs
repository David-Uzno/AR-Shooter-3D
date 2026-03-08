using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Inputs")]
    [SerializeField] private PlayerInput _playerInput;

    [Header("Scenes")]
    [SerializeField] private string _gameOverScene = "GameOver";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public PlayerInput GetPlayerInput()
    {
        return _playerInput;
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene(_gameOverScene);
    }
}
