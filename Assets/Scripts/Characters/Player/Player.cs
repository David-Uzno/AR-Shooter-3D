using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private byte _life = 10;
    private int _score = 0;

    public void TakeDamage(byte damage)
    {
        _life -= damage;
        if (_life <= 0)
        {
            Debug.Log("Player has died");
        }
    }

    public void AddPoints(int points)
    {
        _score += points;
        ScoreText.Instance.UpdateScore(_score);
    }
}
