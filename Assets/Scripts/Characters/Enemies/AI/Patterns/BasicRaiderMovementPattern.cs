using UnityEngine;

public sealed class BasicRaiderMovementPattern : EnemyMovementPatternBase
{
    public override Vector3 GetDesiredPosition(EnemyMovementContext context, EnemyMovementSettings settings, ref EnemyMovementState state)
    {
        BuildPlayerFrame(context.PlayerTransform, out Vector3 forward, out Vector3 right, out Vector3 up);

        float orbitPhase = state.ElapsedTime * settings.MovementSpeed * 0.9f * state.DirectionSign + state.PhaseOffset;
        float forwardDistance = settings.OrbitRadius
            + Mathf.Sin(state.ElapsedTime * 1.35f + state.SecondaryPhaseOffset) * settings.Amplitude * 0.35f
            + state.RadialOffset;

        float lateralOffset = Mathf.Cos(orbitPhase) * settings.Amplitude;
        float verticalOffset = Mathf.Sin(orbitPhase * 1.45f + state.SecondaryPhaseOffset) * settings.Amplitude * 0.6f;

        return context.PlayerTransform.position
            + forward * forwardDistance
            + right * lateralOffset
            + up * verticalOffset;
    }
}
