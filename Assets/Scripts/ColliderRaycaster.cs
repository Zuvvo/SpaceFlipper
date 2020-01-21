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

    static public RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int mask)
    {
        RaycastHit2D[] rayHits = Physics2D.BoxCastAll(origin, size, angle, direction, distance, mask);

        //Setting up the points to draw the cast
        Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
        float w = size.x * 0.5f;
        float h = size.y * 0.5f;
        p1 = new Vector2(-w, h);
        p2 = new Vector2(w, h);
        p3 = new Vector2(w, -h);
        p4 = new Vector2(-w, -h);

        Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        p1 = q * p1;
        p2 = q * p2;
        p3 = q * p3;
        p4 = q * p4;

        p1 += origin;
        p2 += origin;
        p3 += origin;
        p4 += origin;

        Vector2 realDistance = direction.normalized * distance;
        p5 = p1 + realDistance;
        p6 = p2 + realDistance;
        p7 = p3 + realDistance;
        p8 = p4 + realDistance;


        if (rayHits.Length > 0)
        {
            Debug.LogError("RAY BOXCAST HIT");
            RaycastHit2D rayHit = rayHits[0];
            //Drawing the cast
            float time = 3;
            Color castColor = rayHit ? Color.red : Color.green;
            Debug.DrawLine(p1, p2, castColor, time);
            Debug.DrawLine(p2, p3, castColor, time);
            Debug.DrawLine(p3, p4, castColor, time);
            Debug.DrawLine(p4, p1, castColor, time);

            Debug.DrawLine(p5, p6, castColor, time);
            Debug.DrawLine(p6, p7, castColor, time);
            Debug.DrawLine(p7, p8, castColor, time);
            Debug.DrawLine(p8, p5, castColor, time);

            Debug.DrawLine(p1, p5, Color.grey, time);
            Debug.DrawLine(p2, p6, Color.grey, time);
            Debug.DrawLine(p3, p7, Color.grey, time);
            Debug.DrawLine(p4, p8, Color.grey, time);
            if (rayHit)
            {
                Debug.DrawLine(rayHit.point, rayHit.point + rayHit.normal.normalized * 0.2f, Color.yellow);
            }
        }

        return rayHits;
    }
}
