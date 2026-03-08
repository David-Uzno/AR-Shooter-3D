using UnityEngine;

public sealed class SweepOrbitMovementPattern : EnemyMovementPatternBase
{
    public override Vector3 GetDesiredPosition(EnemyMovementContext context, EnemyMovementSettings settings, ref EnemyMovementState state)
    {
        BuildPlayerFrame(context.PlayerTransform, out Vector3 forward, out Vector3 right, out Vector3 up);

        float orbitPhase = state.ElapsedTime * settings.MovementSpeed * 0.65f * state.DirectionSign + state.PhaseOffset;
        float sweepWidth = settings.OrbitRadius + settings.Amplitude;
        float lateralOffset = Mathf.Sin(orbitPhase) * sweepWidth;
        float forwardDistance = settings.MinimumDistance
            + (settings.MaximumDistance - settings.MinimumDistance) * 0.5f
            + Mathf.Cos(orbitPhase) * settings.Amplitude * 0.45f;
        float verticalOffset = Mathf.Sin(orbitPhase * 0.75f + state.SecondaryPhaseOffset) * settings.Amplitude * 0.4f;

        return context.PlayerTransform.position
            + forward * forwardDistance
            + right * lateralOffset
            + up * verticalOffset;
    }
}
