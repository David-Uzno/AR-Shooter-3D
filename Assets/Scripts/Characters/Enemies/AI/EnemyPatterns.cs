using UnityEngine;

public enum EnemyTrajectoryType
{
    BasicRaider = 0,
    SweepOrbit = 1,
    FigureEight = 2
}

public readonly struct EnemyMovementSettings
{
    public float MovementSpeed { get; }
    public float OrbitRadius { get; }
    public float Amplitude { get; }
    public float DirectionChangeFrequency { get; }
    public float MinimumDistance { get; }
    public float MaximumDistance { get; }

    public EnemyMovementSettings(
        float movementSpeed,
        float orbitRadius,
        float amplitude,
        float directionChangeFrequency,
        float minimumDistance,
        float maximumDistance)
    {
        MovementSpeed = movementSpeed;
        OrbitRadius = orbitRadius;
        Amplitude = amplitude;
        DirectionChangeFrequency = directionChangeFrequency;
        MinimumDistance = minimumDistance;
        MaximumDistance = Mathf.Max(minimumDistance, maximumDistance);
    }
}

public readonly struct EnemyMovementContext
{
    public Vector3 EnemyPosition { get; }
    public Transform PlayerTransform { get; }

    public EnemyMovementContext(Vector3 enemyPosition, Transform playerTransform)
    {
        EnemyPosition = enemyPosition;
        PlayerTransform = playerTransform;
    }
}

public struct EnemyMovementState
{
    public float ElapsedTime;
    public float NextDirectionChangeTime;
    public float DirectionSign;
    public float PhaseOffset;
    public float SecondaryPhaseOffset;
    public float RadialOffset;
}

public interface IEnemyMovementPattern
{
    void Initialize(Transform enemyTransform, Transform playerTransform, EnemyMovementSettings settings, ref EnemyMovementState state);
    void Tick(EnemyMovementSettings settings, ref EnemyMovementState state);
    Vector3 GetDesiredPosition(EnemyMovementContext context, EnemyMovementSettings settings, ref EnemyMovementState state);
}
public abstract class EnemyMovementPatternBase : IEnemyMovementPattern
{
    public virtual void Initialize(Transform enemyTransform, Transform playerTransform, EnemyMovementSettings settings, ref EnemyMovementState state)
    {
        state.DirectionSign = Random.value < 0.5f ? -1f : 1f;
        state.PhaseOffset = Random.Range(0f, Mathf.PI * 2f);
        state.SecondaryPhaseOffset = Random.Range(0f, Mathf.PI * 2f);
        state.RadialOffset = Random.Range(-settings.Amplitude * 0.35f, settings.Amplitude * 0.35f);
        state.ElapsedTime = Random.Range(0f, 1.5f);
        state.NextDirectionChangeTime = state.ElapsedTime + GetDirectionChangeInterval(settings);
    }

    public virtual void Tick(EnemyMovementSettings settings, ref EnemyMovementState state)
    {
        state.ElapsedTime += Time.deltaTime;
        if (state.ElapsedTime < state.NextDirectionChangeTime)
        {
            return;
        }

        state.DirectionSign *= -1f;
        state.PhaseOffset += Random.Range(-0.45f, 0.45f);
        state.SecondaryPhaseOffset += Random.Range(-0.7f, 0.7f);
        state.RadialOffset = Random.Range(-settings.Amplitude * 0.5f, settings.Amplitude * 0.5f);
        state.NextDirectionChangeTime = state.ElapsedTime + GetDirectionChangeInterval(settings);
    }

    public abstract Vector3 GetDesiredPosition(EnemyMovementContext context, EnemyMovementSettings settings, ref EnemyMovementState state);

    protected static void BuildPlayerFrame(Transform playerTransform, out Vector3 forward, out Vector3 right, out Vector3 up)
    {
        forward = playerTransform.forward.sqrMagnitude > 0.0001f ? playerTransform.forward.normalized : Vector3.forward;
        up = playerTransform.up.sqrMagnitude > 0.0001f ? playerTransform.up.normalized : Vector3.up;
        right = Vector3.Cross(up, forward).normalized;

        if (right.sqrMagnitude < 0.0001f)
        {
            right = playerTransform.right.sqrMagnitude > 0.0001f ? playerTransform.right.normalized : Vector3.right;
        }
    }

    protected static float GetDirectionChangeInterval(EnemyMovementSettings settings)
    {
        float baseInterval = 1f / Mathf.Max(0.1f, settings.DirectionChangeFrequency);
        return Random.Range(baseInterval * 0.75f, baseInterval * 1.25f);
    }
}
