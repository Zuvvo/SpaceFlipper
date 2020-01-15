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

    public static Vector2 GetOppositeNormalizedVector(this CollisionSide colSide, Vector2 vector)
    {
        vector = vector.normalized;
        switch (colSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                return new Vector2(vector.x, -vector.y);
            case CollisionSide.Left:
            case CollisionSide.Right:
                return new Vector2(-vector.x, vector.y);
            default:
                return Vector2.zero;
        }
    }

    public static void SortByLength(this RaycastHit2D[] rayHits)
    {
        if(rayHits.Length > 1)
        {
            Array.Sort(rayHits, (x, y) => x.distance.CompareTo(y.distance));
        }
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