using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSideDetect
{
    public static CollisionSide GetCollisionSide(Vector3 center, Vector3 contactPoint)
    {
        float xDiff = contactPoint.x - center.x;
        float zDiff = contactPoint.z - center.z;
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