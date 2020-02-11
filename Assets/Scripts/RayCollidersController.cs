using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCollidersController : MonoBehaviour
{
    private List<IRayCollider> rayColliders = new List<IRayCollider>();
    private List<IRayCollider> collisions = new List<IRayCollider>();

    private WaitForFixedUpdate _wait = new WaitForFixedUpdate();

    private static RayCollidersController instance;
    public static RayCollidersController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RayCollidersController>();
            }
            return instance;
        }
    }

    private void Start()
    {
        StartCoroutine(FixedUpdateCoroutine());
    }

    private IEnumerator FixedUpdateCoroutine()
    {
        while (true)
        {
            yield return _wait;
            for (int i = 0; i < rayColliders.Count; i++)
            {
                IRayCollider rayCollider = rayColliders[i];
                rayCollider.Raycast();
                if (collisions.Contains(rayCollider))
                {
                    rayCollider.HandleCollision(new List<IRayCollider>());
                }
                rayCollider.OnFixedUpdateTick();
            }
            Clear();
        }
    }

    private void FixedUpdate()
    {
        //for (int i = 0; i < rayColliders.Count; i++)
        //{
        //    IRayCollider rayCollider = rayColliders[i];
        //    rayCollider.Raycast();
        //    if (collisions.Contains(rayCollider))
        //    {
        //        rayCollider.HandleCollision(new List<IRayCollider>());
        //    }
        //    rayCollider.OnFixedUpdateTick();
        //}
        //Clear();
    }

    private void CastRays()
    {
        for (int i = 0; i < rayColliders.Count; i++)
        {
            rayColliders[i].Raycast();
        }
    }

    private void HandleCollisions()
    {
        for (int i = 0; i < collisions.Count; i++)
        {
           List<IRayCollider> colliededWith = collisions[i].HandleCollision(new List<IRayCollider>());
        }
    }

    private void UpdateObjects()
    {
        for (int i = 0; i < rayColliders.Count; i++)
        {
            rayColliders[i].OnFixedUpdateTick();
        }
    }

    private void Clear()
    {
        collisions.Clear();
    }

    public void RegisterCollision(IRayCollider rayCollider, RaycastHit2D rayHit)
    {
        collisions.Add(rayCollider);
    }

    public void RegisterRayCollider(IRayCollider rayCollider, bool addAsFirst = false)
    {
        if (!rayColliders.Contains(rayCollider))
        {
            if (addAsFirst)
            {
                rayColliders.Insert(0, rayCollider);
            }
            else
            {
                rayColliders.Add(rayCollider);
            }
        }
    }

    public void UnregisterRayCollider(IRayCollider rayCollider)
    {
        if (rayColliders.Contains(rayCollider))
        {
            rayColliders.Remove(rayCollider);
        }
    }
}