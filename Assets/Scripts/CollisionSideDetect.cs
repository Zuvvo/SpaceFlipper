using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSideDetect
{
    public static CollisionSide GetCollisionSide(Vector2 center, Vector2 contactPoint)
    {
        float xDiff = contactPoint.x - center.x;
        float yDiff = contactPoint.y - center.y;
        if(Mathf.Abs(xDiff) > Mathf.Abs(yDiff))
        {
            return xDiff < 0 ? CollisionSide.Left : CollisionSide.Right;
        }
        else
        {
            return yDiff < 0 ? CollisionSide.Bottom : CollisionSide.Top;
        }
    }

    public static CollisionSide GetCollisionSideBasedOnTriangleAndBottomPoint(Vector2 leftPoint, Vector2 rightPoint, Vector2 bottomPoint, Vector2 targetPoint)
    {
        Vector2 leftToRightVector = rightPoint - leftPoint;
        Vector2 leftToTargetVector = targetPoint - leftPoint;
        float leftToTargetAngle = Vector2.Angle(leftToTargetVector.normalized, leftToRightVector.normalized);

        if(leftToTargetAngle >= 120)
        {
            return CollisionSide.Right;
        }

        Vector2 rightToLeftVector = leftPoint - rightPoint;
        Vector2 rightToTargetVector = targetPoint - rightPoint;
        float rightToTargetAngle = Vector2.Angle(rightToTargetVector.normalized, rightToLeftVector.normalized);

        if (rightToTargetAngle >= 120)
        {
            return CollisionSide.Left;
        }

        Vector2 centerPoint = (rightPoint + leftPoint) / 2;
        float objectCenterToTargetLength = (targetPoint - centerPoint).magnitude;
        float bottomToTargetLength = (targetPoint - bottomPoint).magnitude;

        if(objectCenterToTargetLength < bottomToTargetLength)
        {
            return CollisionSide.Bottom;
        }
        else
        {
            return CollisionSide.Top;
        }
    }

}