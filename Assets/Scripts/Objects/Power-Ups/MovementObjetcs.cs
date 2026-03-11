using UnityEngine;

public class MovementObjetcs : MonoBehaviour
{
    [Header("Ubication")]
    [SerializeField] private float _targetY = 0f;
    private float _reachThreshold = 0.01f;
    [SerializeField] private bool _useLocalPosition = false;

    [Header("Speed")]    
    private float _speed;
    [SerializeField] private float _minSpeed = 0.5f;
    [SerializeField] private float _maxSpeed = 2f;
    
    

    private void OnEnable()
    {
        if (_minSpeed > _maxSpeed)
        {
            (_maxSpeed, _minSpeed) = (_minSpeed, _maxSpeed);
        }

        _speed = Random.Range(_minSpeed, _maxSpeed);
    }

    private void Update()
    {
        Vector3 currentPosition = _useLocalPosition ? transform.localPosition : transform.position;
        float newY = Mathf.MoveTowards(currentPosition.y, _targetY, _speed * Time.deltaTime);

        if (_useLocalPosition)
            transform.localPosition = new Vector3(currentPosition.x, newY, currentPosition.z);
        else
            transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);

        if (Mathf.Abs(newY - _targetY) <= _reachThreshold)
            gameObject.SetActive(false);
    }
}
