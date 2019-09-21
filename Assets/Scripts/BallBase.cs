using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBase : MonoBehaviour
{
    public TrailRenderer FastSpeedTrail;
    public TrailRenderer SlowSpeedTrail;
    public Rigidbody Rigidbody;
    public Collider Collider;
    public Vector3 LastFrameCenterPoint { get; private set; }

    private float minVel = 0.1f;
    private float maxVel = 0.5f;

    private Vector3 lastFrameVelocity;

    private bool isFastTrailOn;
    private const float speedThreshold = 1.5f;

    private void Start()
    {
        Rigidbody.AddForce(new Vector3(-1, 0, 0), ForceMode.Impulse);
    }

    public void Stop()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        lastFrameVelocity = Rigidbody.velocity;
        LastFrameCenterPoint = Collider.bounds.center;
        SetTrailBasedOnSpeed();
    }

    private void SetTrailBasedOnSpeed()
    {
        isFastTrailOn = Rigidbody.velocity.magnitude >= speedThreshold;
        FastSpeedTrail.emitting = isFastTrailOn;
        SlowSpeedTrail.emitting = !isFastTrailOn;
    }

    public void SetOppositeVelocity(CollisionSide colliderSide)
    {
        float xVel = lastFrameVelocity.x;
        float zVel = lastFrameVelocity.z;
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                Rigidbody.velocity = new Vector3(-xVel, 0, zVel).normalized;
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                Rigidbody.velocity = new Vector3(xVel, 0, -zVel).normalized;
                break;
        }
    }

    public void AddForceOnStrikerHit(Vector3 leftStrikeForceVector)
    {
        Rigidbody.velocity = lastFrameVelocity + leftStrikeForceVector;
    }
}