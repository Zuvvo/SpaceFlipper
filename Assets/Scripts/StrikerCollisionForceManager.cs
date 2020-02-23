using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerCollisionForceManager : MonoBehaviour
{
    private static Vector2 leftStrikerPowerHitForce = new Vector2(-1.35f, 2.7f);
    private static Vector2 rightStrikerPowerHitForce = new Vector2(1.35f, 2.7f);

    private static Vector2 leftStrikerIdleHitForce = new Vector2(-0.9f, 1.8f);
    private static Vector2 rightStrikerIdleHitForce = new Vector2(0.9f, 1.8f);

    private static Vector2 leftStrikerMovedUpHitForce = new Vector2(0.9f, 1.8f);
    private static Vector2 rightStrikerMovedUpHitForce = new Vector2(-0.9f, 1.8f);

    private static Vector2 leftSideMovedUpHitForce = new Vector2(-1.35f, 1.35f);
    private static Vector2 rightSideMovedUpHitForce = new Vector2(1.35f, 1.35f);

    private static Vector2 leftSideIdleHitForce = new Vector2(-1.35f, -1.35f);
    private static Vector2 rightSideIdleHitForce = new Vector2(1.35f, -1.35f);

    private static Vector2 leftBottomIdleHitForce = new Vector2(0.9f, -1.35f);
    private static Vector2 leftBottomMovedUpHitForce = new Vector2(-0.9f, -1.35f);

    private static Vector2 rightBottomIdleHitForce = new Vector2(-0.9f, -1.35f);
    private static Vector2 rightBottomMovedUpHitForce = new Vector2(0.9f, -1.35f);

    public static Vector2 GetCollisionEndVelocity(CollisionType colType, CollisionSide colSide, bool isMovingOrMovedUp, bool isForceModeOn)
    {
        switch (colSide)
        {
            case CollisionSide.Bottom:
                if (isMovingOrMovedUp)
                {
                    return colType == CollisionType.StrikerLeft ? leftBottomMovedUpHitForce : rightBottomMovedUpHitForce;
                }
                else
                {
                    return colType == CollisionType.StrikerLeft ? leftBottomIdleHitForce : rightBottomIdleHitForce;
                }
            case CollisionSide.Top:
                if (isForceModeOn)
                {
                    return colType == CollisionType.StrikerLeft ? leftStrikerPowerHitForce : rightStrikerPowerHitForce;
                }
                else
                {
                    if (isMovingOrMovedUp)
                    {
                        return colType == CollisionType.StrikerLeft ? leftStrikerMovedUpHitForce : rightStrikerMovedUpHitForce;
                    }
                    else
                    {
                        return colType == CollisionType.StrikerLeft ? leftStrikerIdleHitForce : rightStrikerIdleHitForce;
                    }
                }
            case CollisionSide.Left:
            case CollisionSide.Right:
                if (colSide == CollisionSide.Left)
                {
                    return isMovingOrMovedUp ? rightSideMovedUpHitForce : rightSideIdleHitForce;
                }
                else if (colSide == CollisionSide.Right)
                {
                    return isMovingOrMovedUp ? leftSideMovedUpHitForce : leftSideIdleHitForce;
                }
                else
                {
                    Debug.LogError("RETURN VECTOR2 UP COL STRIKER LEFT/RIGHT");
                    return Vector2.up;
                }
        }
        return Vector2.up;
    }
}