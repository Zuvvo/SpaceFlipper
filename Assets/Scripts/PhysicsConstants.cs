using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsConstants
{
    private const float m = 5; //global multiplier

    
    public const float BallSpeedAtStart = m* 1;
    public const float BallSpeedAfterEnemyHit = m *1;
    public const float BallSpeedAfterBallHit = m * 1;
    public const float BallSpeedAfterStrikerIdleHit = m * 1;
    public const float BallSpeedAfterShipHit = m * 1;
    public const float BallSpeedAfterMovingShipHit = m * 1; //todo
    public const float BallSpeedAfterStrikerForceHit = m * 2.2f;
    public const float BallSpeedAfterStrikerIdleMovingHit = m * 1.5f; //todo
    public const float BallSpeedPowerShotThreshold = m * 1.7f;
}