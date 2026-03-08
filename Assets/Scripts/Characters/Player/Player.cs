using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : Character
{
    [Header("Dependencies")]
    [SerializeField] private Image _lifeBarUI;
    [SerializeField] private Shooting _shooting;
    private PlayerInput _playerInput;

    private int _score = 0;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            _playerInput = GameManager.Instance.GetPlayerInput();
        }
    }

    protected override void Start()
    {
        base.Start();

        if (_playerInput == null && GameManager.Instance != null)
        {
            _playerInput = GameManager.Instance.GetPlayerInput();
        }
    }

    private void Update()
    {
        if (_playerInput != null && _shooting != null)
        {
            if (_playerInput.actions["Interact"].WasPressedThisFrame())
            {
                _shooting.Fire(transform.forward);
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (_lifeBarUI != null)
        {
            UpdateLifeBar();
        }

        if (_lifeCurrent <= 0)
        {
            Debug.Log("Muerte");
        }
    }

    private void UpdateLifeBar()
    {
        _lifeBarUI.fillAmount = GetLifeNormalized();
    }

    protected float GetLifeNormalized()
    {
        if (_lifeMax == 0) return 0f;
        return (float)_lifeCurrent / _lifeMax;
    }

    public void AddPoints(int points)
    {
        _score += points;
        ScoreText.Instance.UpdateScore(_score);
    }
}
