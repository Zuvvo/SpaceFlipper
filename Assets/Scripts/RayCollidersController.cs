using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCollidersController : MonoBehaviour
{
    private List<IRayCollider> rayColliders = new List<IRayCollider>();
    private List<IRayCollider> collisions = new List<IRayCollider>();

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

    private void Update()
    {
        CastRays();
        HandleCollisions();
        UpdateObjects();
        Clear();
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
           List<IRayCollider> colliededWith = collisions[i].RayCollision(new List<IRayCollider>());
        }
    }

    private void UpdateObjects()
    {
        for (int i = 0; i < rayColliders.Count; i++)
        {
            rayColliders[i].OnUpdate();
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

    public void RegisterRayCollider(IRayCollider rayCollider)
    {
        if (!rayColliders.Contains(rayCollider))
        {
            rayColliders.Add(rayCollider);
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