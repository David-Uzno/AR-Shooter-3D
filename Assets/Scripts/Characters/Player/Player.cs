using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private byte _life = 10;
    private byte _lifeMax;
    private byte _lifeCurrent;

    [Header("Dependencies")]
    [SerializeField] private Image _lifeBarUI;

    private int _score = 0;

    private void Start()
    {
        _lifeMax = _life;
        _lifeCurrent = _lifeMax;
    }

    public void TakeDamage(byte damage)
    {
        _lifeCurrent -= damage;

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
        _lifeBarUI.fillAmount = (float)_lifeCurrent / _lifeMax;
    }

    public void AddPoints(int points)
    {
        _score += points;
        ScoreText.Instance.UpdateScore(_score);
    }
}
