using UnityEngine;

public sealed class FigureEightMovementPattern : EnemyMovementPatternBase
{
    public override Vector3 GetDesiredPosition(EnemyMovementContext context, EnemyMovementSettings settings, ref EnemyMovementState state)
    {
        BuildPlayerFrame(context.PlayerTransform, out Vector3 forward, out Vector3 right, out Vector3 up);

        float orbitPhase = state.ElapsedTime * settings.MovementSpeed * 0.8f * state.DirectionSign + state.PhaseOffset;
        float forwardDistance = settings.OrbitRadius
            + Mathf.Cos(orbitPhase + state.SecondaryPhaseOffset) * settings.Amplitude * 0.3f
            + state.RadialOffset;

        float lateralOffset = Mathf.Sin(orbitPhase) * settings.Amplitude * 1.15f;
        float verticalOffset = Mathf.Sin(orbitPhase * 2f + state.SecondaryPhaseOffset) * settings.Amplitude * 0.7f;

        return context.PlayerTransform.position
            + forward * forwardDistance
            + right * lateralOffset
            + up * verticalOffset;
    }
}
