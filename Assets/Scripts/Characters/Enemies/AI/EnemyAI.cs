using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyAIData _enemyAIConfiguration;
    [SerializeField] private Transform _playerTransform;

    private IEnemyMovementPattern _movementPattern;
    private EnemyMovementState _movementState;
    private bool _isInitialized;

    private void Start()
    {
        if (_enemyAIConfiguration == null)
        {
            Debug.LogError($"{nameof(EnemyAI)} en {name} no tiene asignado un ScriptableObject de tipo {nameof(EnemyAIData)}.", this);
            enabled = false;
            return;
        }

        if (!TryResolvePlayerTransform())
        {
            enabled = false;
            return;
        }

        EnemyMovementSettings settings = BuildSettings();
        _movementPattern = EnemyPatternFactory.Create(_enemyAIConfiguration.TrajectoryType);
        _movementPattern.Initialize(transform, _playerTransform, settings, ref _movementState);
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized || _playerTransform == null || _enemyAIConfiguration == null)
        {
            return;
        }

        EnemyMovementSettings settings = BuildSettings();
        _movementPattern.Tick(settings, ref _movementState);

        EnemyMovementContext context = new(transform.position, _playerTransform);
        Vector3 desiredPosition = _movementPattern.GetDesiredPosition(context, settings, ref _movementState);
        desiredPosition = ClampDistanceToPlayer(desiredPosition, settings);

        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, settings.MovementSpeed * Time.deltaTime);

        if (_enemyAIConfiguration.LookAtPlayer)
        {
            RotateTowardsPlayer();
        }
    }

    public void SetPlayerTransform(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }

    private bool TryResolvePlayerTransform()
    {
        if (_playerTransform != null)
        {
            return true;
        }

        Player player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning($"{nameof(EnemyAI)} en {name} no pudo resolver el Transform del jugador durante la inicialización.", this);
            return false;
        }

        _playerTransform = player.transform;
        return true;
    }

    private EnemyMovementSettings BuildSettings()
    {
        return new EnemyMovementSettings(
            _enemyAIConfiguration.MovementSpeed,
            _enemyAIConfiguration.MovementRadius,
            _enemyAIConfiguration.MovementAmplitude,
            _enemyAIConfiguration.DirectionChangeFrequency,
            _enemyAIConfiguration.MinimumDistance,
            _enemyAIConfiguration.MaximumDistance);
    }

    private Vector3 ClampDistanceToPlayer(Vector3 desiredPosition, EnemyMovementSettings settings)
    {
        Vector3 offset = desiredPosition - _playerTransform.position;
        if (offset.sqrMagnitude < 0.0001f)
        {
            if (_playerTransform.forward.sqrMagnitude > 0.0001f)
            {
                offset = _playerTransform.forward.normalized * settings.MinimumDistance;
            }
            else
            {
                offset = Vector3.forward * settings.MinimumDistance;
            }
        }

        float clampedDistance = Mathf.Clamp(offset.magnitude, settings.MinimumDistance, settings.MaximumDistance);
        return _playerTransform.position + offset.normalized * clampedDistance;
    }

    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = _playerTransform.position - transform.position;
        if (directionToPlayer.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
    }
}
