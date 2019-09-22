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

    private readonly float allowToHitTimeReset = 0.2f;
    private float allowToHitTimer = 0;

    private bool isFastTrailOn;
    private const float speedThreshold = 1.7f;
    private bool wasHittedRecently
    {
        get { return hittedRecently_; }
        set
        {
            allowToHitTimer = 0;
            hittedRecently_ = value;
        }
    }
    private bool hittedRecently_;

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
        if (!wasHittedRecently)
        {
            lastFrameVelocity = Rigidbody.velocity;
        }
        LastFrameCenterPoint = Collider.bounds.center;
        SetHittedState();
        SetTrailBasedOnSpeed();
    }

    private void SetHittedState()
    {
        if (wasHittedRecently)
        {
            allowToHitTimer += Time.deltaTime;
            if(allowToHitTimer >= allowToHitTimeReset)
            {
                wasHittedRecently = false;
            }
        }
        else
        {
            allowToHitTimer = 0;
        }
    }

    private void SetTrailBasedOnSpeed()
    {
        isFastTrailOn = Rigidbody.velocity.magnitude >= speedThreshold;
        FastSpeedTrail.emitting = isFastTrailOn;
        SlowSpeedTrail.emitting = !isFastTrailOn;
    }

    public void SetOppositeVelocity(CollisionSide colliderSide, float endSpeed)
    {
        float xVel = lastFrameVelocity.x;
        float zVel = lastFrameVelocity.z;
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                Rigidbody.velocity = new Vector3(-xVel, 0, zVel).normalized * endSpeed;
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                Rigidbody.velocity = new Vector3(xVel, 0, -zVel).normalized * endSpeed;
                break;
        }
    }

    public void SetSpeedOnBallCollisionExit()
    {
        Rigidbody.velocity = Rigidbody.velocity.normalized * PhysicsConstants.BallSpeedAfterBallHit;
        lastFrameVelocity = Rigidbody.velocity;
    }

    public void SetLastFrameVelocityOnCollisionStay()
    {
       // Debug.Log(gameObject.name + " on collision stay before: " + Rigidbody.velocity);
        Rigidbody.velocity = lastFrameVelocity;
        lastFrameVelocity = Rigidbody.velocity;
       // Debug.Log(gameObject.name + " on collision stay after: " + Rigidbody.velocity);
    }

    public void SetLastFrameVelocityOnCollisionExit()
    {
       // Debug.Log(gameObject.name + " on collision EXIT before: " + Rigidbody.velocity);
        Rigidbody.velocity = lastFrameVelocity;
        lastFrameVelocity = Rigidbody.velocity;
       // Debug.Log(gameObject.name + " on collision EXIT after: " + Rigidbody.velocity);
    }

    public void AddForceOnStrikerHit(Vector3 forceVector, float endSpeed)
    {
        if (!wasHittedRecently)
        {
            Vector3 finalVelocityVector = (lastFrameVelocity.normalized + forceVector).normalized * endSpeed;
            Rigidbody.velocity = finalVelocityVector;
           // Debug.LogWarningFormat("[{0}]{1} + {2} = {3} || speed: {4}", gameObject.name, lastFrameVelocity.normalized, forceVector, finalVelocityVector, Rigidbody.velocity.magnitude);
            lastFrameVelocity = Rigidbody.velocity;
            wasHittedRecently = true;
        }
        else
        {
            Rigidbody.velocity = lastFrameVelocity;
            wasHittedRecently = true;
            Debug.LogErrorFormat("[{0}]BAD HIT!!! {1}", gameObject.name, Rigidbody.velocity);
        }
    }

    public void AddForceOnShipHit()
    {
        if (!wasHittedRecently)
        {
            float xVel = lastFrameVelocity.x;
            float zVel = lastFrameVelocity.z;
            Rigidbody.velocity = new Vector3(-xVel, 0, zVel).normalized;
            lastFrameVelocity = Rigidbody.velocity;
            wasHittedRecently = true;
        }
        else
        {
            Rigidbody.velocity = lastFrameVelocity;
            wasHittedRecently = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.collider.GetComponent<BallBase>();
            if (ball != null)
            {
                ball.SetSpeedOnBallCollisionExit();
                SetSpeedOnBallCollisionExit();
            }
        }
    }
}