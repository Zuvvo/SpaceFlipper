using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBase : MonoBehaviour
{
    public Rigidbody RigidBody;
    public Collider Collider;
    public Vector3 LastFrameCenterPoint { get; private set; }

    private float minVel = 0.1f;
    private float maxVel = 0.5f;

    private Vector3 lastFrameVelocity;

    private void Start()
    {
        RigidBody.AddForce(new Vector3(-1, 0, 0), ForceMode.Impulse);
    }

    public void Stop()
    {
        RigidBody.velocity = Vector3.zero;
        RigidBody.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        lastFrameVelocity = RigidBody.velocity;
        LastFrameCenterPoint = Collider.bounds.center;
    }


    public void SetOppositeVelocity(CollisionSide colliderSide)
    {
        float xVel = lastFrameVelocity.x;
        float zVel = lastFrameVelocity.z;
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                RigidBody.velocity = new Vector3(-xVel, 0, zVel).normalized;
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                RigidBody.velocity = new Vector3(xVel, 0, -zVel).normalized;
                break;

        }
    }
}