using System;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Vector2 GetCollisionDirectionVector(this CollisionSide colSide, Vector2 vector)
    {
        float xVel = vector.x;
        float yVel = vector.y;

        float xVelAbs = Mathf.Abs(xVel);
        float yVelAbs = Mathf.Abs(yVel);

        Vector2 endVector = new Vector2();
        switch (colSide)
        {
            case CollisionSide.Bottom:
                endVector = new Vector2(xVel, yVelAbs);
                break;
            case CollisionSide.Top:
                endVector = new Vector2(xVel, -yVelAbs);
                break;
            case CollisionSide.Left:
                endVector = new Vector2(xVelAbs, yVel);
                break;
            case CollisionSide.Right:
                endVector = new Vector2(-xVelAbs, yVel);
                break;
        }

        return endVector.normalized;
    }

    public static void SortByLength(this RaycastHit2D[] rayHits)
    {
        if(rayHits.Length > 1)
        {
            Array.Sort(rayHits, (x, y) => x.distance.CompareTo(y.distance));
        }
    }

    public static RaycastHit2D[] Remove(this RaycastHit2D[] rayHits, RaycastHit2D ray)
    {
        if(rayHits.Length > 0)
        {
            List<RaycastHit2D> result = new List<RaycastHit2D>();
            for (int i = 0; i < rayHits.Length; i++)
            {
                RaycastHit2D rayHit = rayHits[i];
                if(rayHit.transform.GetInstanceID() != ray.transform.GetInstanceID())
                {
                    result.Add(rayHit);
                }
            }
            return result.ToArray();
        }
        return rayHits;
    }

    public static Vector2 GetOppositeDirectionVector(this CollisionSide colSide)
    {
        switch (colSide)
        {
            case CollisionSide.Bottom:
                return Vector2.up;
            case CollisionSide.Top:
                return Vector2.down;
            case CollisionSide.Left:
                return Vector2.right;
            case CollisionSide.Right:
                return Vector2.left;
        }
        return Vector2.zero;
    }
}