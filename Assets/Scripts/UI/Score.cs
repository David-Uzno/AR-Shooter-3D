using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private int _points = 50;

    private int _currentScore = 0;
    public static Score Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints()
    {
        _currentScore += _points;
        _score.text = _currentScore.ToString();
    }
}
