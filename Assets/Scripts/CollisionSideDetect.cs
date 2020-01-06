using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSideDetect
{
    public static CollisionSide GetCollisionSide(Vector2 center, Vector2 contactPoint)
    {
        float xDiff = contactPoint.x - center.x;
        float zDiff = contactPoint.y - center.y;
        if(Mathf.Abs(xDiff) > Mathf.Abs(zDiff))
        {
            return xDiff < 0 ? CollisionSide.Top : CollisionSide.Bottom;
        }
        else
        {
            return zDiff < 0 ? CollisionSide.Left : CollisionSide.Right;
        }
    }

}