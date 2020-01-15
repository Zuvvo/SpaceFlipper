using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderRaycaster : MonoBehaviour
{
    public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, out RayData rayData)
    {
        rayData = new RayData();
        List<RaycastHit2D> rayHits = new List<RaycastHit2D>();

        RaycastHit2D[] rays = Physics2D.CircleCastAll(origin, radius, direction, distance, layerMask);

        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit2D rayHit = rays[i];
        }

        return rayHits.ToArray();
    }
}
