using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBase : MonoBehaviour
{
    public TrailRenderer FastSpeedTrail;
    public TrailRenderer SlowSpeedTrail;
    public Rigidbody2D Rigidbody;
    public CircleCollider2D Collider;
    public Vector2 LastFrameCenterPoint { get; private set; }

    private float minVel = 0.1f;
    private float maxVel = 0.5f;

    public Vector2 LastFrameVelocity { get; private set; }

    private readonly float allowToHitTimeReset = 0.2f;
    private float allowToHitTimer = 0;

    private bool isFastTrailOn;
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
        Debug.Log((int)Rigidbody.constraints);
    }

    public void Stop()
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;
    }

    public void SetGravityState(bool state)
    {
        Rigidbody.gravityScale = state ? 1 : 0;
        enabled = !state;
    }

    private void Update()
    {
        if (!wasHittedRecently)
        {
            LastFrameVelocity = Rigidbody.velocity;
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
       // Debug.Log(Rigidbody.velocity.magnitude);
        isFastTrailOn = Rigidbody.velocity.magnitude >= PhysicsConstants.BallSpeedPowerShotThreshold;
        FastSpeedTrail.emitting = isFastTrailOn;
        SlowSpeedTrail.emitting = !isFastTrailOn;
    }

    public void SetOppositeVelocity(CollisionSide colliderSide)
    {
        float xVel = LastFrameVelocity.x;
        float yVel = LastFrameVelocity.y;
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                Rigidbody.velocity = new Vector2(-xVel, yVel);
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                Rigidbody.velocity = new Vector2(xVel, -yVel);
                break;
        }
    }

    public void SetOppositeVelocity(CollisionSide colliderSide, float endSpeed)
    {
        float xVel = LastFrameVelocity.x;
        float yVel = LastFrameVelocity.y;
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                Rigidbody.velocity = new Vector2(-xVel, yVel).normalized * endSpeed;
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                Rigidbody.velocity = new Vector2(xVel, -yVel).normalized * endSpeed;
                break;
        }
    }

    public void SetSpeedOnBallCollisionExit()
    {
        Rigidbody.velocity = Rigidbody.velocity.normalized * PhysicsConstants.BallSpeedAfterBallHit;
        LastFrameVelocity = Rigidbody.velocity;
    }

    public void SetLastFrameVelocityOnCollisionStay()
    {
        //Debug.Log(gameObject.name + " on collision stay before: " + Rigidbody.velocity);
        Rigidbody.velocity = LastFrameVelocity;
        LastFrameVelocity = Rigidbody.velocity;
        //Debug.Log(gameObject.name + " on collision stay after: " + Rigidbody.velocity);
    }

    public void SetLastFrameVelocityOnCollisionExit()
    {
       // Debug.Log(gameObject.name + " on collision EXIT before: " + Rigidbody.velocity);
        Rigidbody.velocity = LastFrameVelocity;
        LastFrameVelocity = Rigidbody.velocity;
       // Debug.Log(gameObject.name + " on collision EXIT after: " + Rigidbody.velocity);
    }

    public void AddForceOnStrikerHit(Vector2 forceVector, float endSpeed)
    {
        if (!wasHittedRecently)
        {
            Vector2 finalVelocityVector = (LastFrameVelocity.normalized + forceVector).normalized * endSpeed;
            Rigidbody.velocity = finalVelocityVector;
            Debug.LogWarningFormat("[{0}]{1} + {2} = {3} || speed: {4}", gameObject.name, LastFrameVelocity.normalized, forceVector, finalVelocityVector, Rigidbody.velocity.magnitude);
            LastFrameVelocity = Rigidbody.velocity;
            wasHittedRecently = true;
        }
        else
        {
            Rigidbody.velocity = LastFrameVelocity;
            wasHittedRecently = true;
            Debug.LogErrorFormat("[{0}]BAD HIT!!! {1}", gameObject.name, Rigidbody.velocity);
        }
    }

    public void AddForceOnShipHit(CollisionSide colSide)
    {
        if (!wasHittedRecently)
        {
            float xVel = LastFrameVelocity.x;
            float yVel = LastFrameVelocity.y;
            Rigidbody.velocity = new Vector2(xVel, -yVel).normalized * PhysicsConstants.BallSpeedAfterShipHit;
            LastFrameVelocity = Rigidbody.velocity;
        }
        else
        {
            Rigidbody.velocity = LastFrameVelocity;
        }
        wasHittedRecently = true;
    }
    
    private void OnCollisionExit2D(Collision2D collision)
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