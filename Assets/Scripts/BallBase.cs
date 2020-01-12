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
    public Vector2 LastFrameVelocity { get; private set; }

    private Vector2 currentCenterPoint;

    public LayerMask ColliderLayerMask;

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
        currentCenterPoint = transform.position;
        if (!RaycastForColliders())
        {
            LastFrameVelocity = Rigidbody.velocity;
            LastFrameCenterPoint = transform.position;
          //  Debug.LogFormat("set last frame pos: {0} -- {1}", LastFrameCenterPoint.x, LastFrameCenterPoint.y);
        }
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

    public void ResetLastFrameVelocityAndPosition()
    {
        LastFrameVelocity = Vector2.zero;
        LastFrameCenterPoint = transform.position;
    }

    public void SetOppositeVelocity(CollisionSide colliderSide, bool useLastFrameVelocity = false)
    {
        //Debug.Log(LastFrameCenterPoint);
       // transform.position = LastFrameCenterPoint;
        float xVel = useLastFrameVelocity ? LastFrameVelocity.x : Rigidbody.velocity.x;
        float yVel = useLastFrameVelocity ? LastFrameVelocity.y : Rigidbody.velocity.y;
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                Rigidbody.velocity = new Vector2(xVel, -yVel);
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                Rigidbody.velocity = new Vector2(-xVel, yVel);
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

    private bool RaycastForColliders()
    {
        if(LastFrameCenterPoint == currentCenterPoint)
        {
            return false;
        }

        Vector2 direction = currentCenterPoint - LastFrameCenterPoint;
        float distance = direction.magnitude;
        float radius = Collider.radius / 2;
        RaycastHit2D[] rayHits = Physics2D.CircleCastAll(LastFrameCenterPoint, radius, direction, distance, ColliderLayerMask);
      //  Debug.DrawLine(transform.position, LastFrameCenterPoint, Color.green, 3);

        for (int i = 0; i < rayHits.Length; i++)
        {
            RaycastHit2D rayHit = rayHits[i];
            if (rayHit.collider.CompareTag(GameTags.Frame))
            {
                Debug.DrawLine(currentCenterPoint, LastFrameCenterPoint, Color.green, 2);
                FrameCollider frameCollider = rayHit.collider.GetComponent<FrameCollider>();
                if (frameCollider.DestroyBallAndProjectiles)
                {
                    GameController.Instance.RemoveBallFromPlay(this);
                }
                else
                {
                   // Debug.LogFormat("Frame hit. last frame point: {0} -- {1} this frame point: {2} -- {3}", LastFrameCenterPoint.x, LastFrameCenterPoint.y, currentCenterPoint.x, currentCenterPoint.y);
                    //transform.position = rayHit.point;
                    Vector2 newPos = rayHit.point + frameCollider.CollisionSide.GetOppositeDirectionVector() * radius; ;
                    transform.position = newPos;
                   // Debug.LogFormat("new pos: {0} -- {1}", newPos.x, newPos.y);
                    LastFrameCenterPoint = newPos;
                    SetOppositeVelocity(frameCollider.CollisionSide);
                    LastFrameVelocity = Rigidbody.velocity;
                }
                return true;
            }
        }
        return false;
    }
}