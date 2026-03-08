using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyConfiguration", menuName = "AR-Shooter/AI/Enemy AI Configuration")]
public class EnemyAIData : ScriptableObject
{
    [Header("Movement Properties")]
    [SerializeField] [Min(0.1f)] private float _movementSpeed = 3f;
    [SerializeField] [Min(0.5f)] private float _movementRadius = 3f;
    [SerializeField] [Min(0f)] private float _movementAmplitude = 1.25f;
    [SerializeField] [Min(0.1f)] private float _directionChangeFrequency = 0.9f;
    [SerializeField] [Min(0.5f)] private float _minimumDistance = 2f;
    [SerializeField] [Min(0.5f)] private float _maximumDistance = 6f;
    
    [Header("Trajectory Settings")]
    [SerializeField] private EnemyTrajectoryType _trajectoryType = EnemyTrajectoryType.BasicRaider;
    [SerializeField] private bool _lookAtPlayer = true;

    public float MovementSpeed => _movementSpeed;
    public float MovementRadius => _movementRadius;
    public float MovementAmplitude => _movementAmplitude;
    public float DirectionChangeFrequency => _directionChangeFrequency;
    public float MinimumDistance => _minimumDistance;
    public float MaximumDistance => _maximumDistance;
    public EnemyTrajectoryType TrajectoryType => _trajectoryType;
    public bool LookAtPlayer => _lookAtPlayer;

    private void OnValidate()
    {
        _movementSpeed = Mathf.Max(0.1f, _movementSpeed);
        _movementRadius = Mathf.Max(0.5f, _movementRadius);
        _movementAmplitude = Mathf.Max(0f, _movementAmplitude);
        _directionChangeFrequency = Mathf.Max(0.1f, _directionChangeFrequency);
        _minimumDistance = Mathf.Max(0.5f, _minimumDistance);
        _maximumDistance = Mathf.Max(_minimumDistance, _maximumDistance);
    }
}
