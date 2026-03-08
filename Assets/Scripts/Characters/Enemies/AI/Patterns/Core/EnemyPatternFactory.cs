using UnityEngine;

public static class EnemyPatternFactory
{
    public static IEnemyMovementPattern Create(EnemyTrajectoryType trajectoryType)
    {
        return trajectoryType switch
        {
            EnemyTrajectoryType.SweepOrbit => new SweepOrbitMovementPattern(),
            EnemyTrajectoryType.FigureEight => new FigureEightMovementPattern(),
            _ => new BasicRaiderMovementPattern(),
        };
    }
}
