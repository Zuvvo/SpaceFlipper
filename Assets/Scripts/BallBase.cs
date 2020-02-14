using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBase : MonoBehaviour, IRayCollider
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

    private RaycastHit2D[] rayHits;
    private Striker striker;

    private bool skipOneFrameFromCollisionSystem;

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
        RegisterObject();
    }

    private void OnDestroy()
    {
        Unregister();
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

    private void SetHittedState()
    {
        if (wasHittedRecently)
        {
            allowToHitTimer += Time.deltaTime;
            if (allowToHitTimer >= allowToHitTimeReset)
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

    public void ResetTrailEmitter()
    {
        FastSpeedTrail.emitting = false;
        SlowSpeedTrail.emitting = false;
    }

    public void ResetLastFrameVelocityAndPosition()
    {
        LastFrameVelocity = Vector2.zero;
        LastFrameCenterPoint = transform.position;
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

    public void RaycastAndSkipThisFrameCollision()
    {
        //Debug.LogError("STOP");
        RaycastForColliders();
        HandleCollision(new List<IRayCollider>());
        OnFixedUpdateTick();
        skipOneFrameFromCollisionSystem = true;
    }

    private void RaycastForColliders()
    {
        if (skipOneFrameFromCollisionSystem)
        {
            return;
        }

        currentCenterPoint = transform.position;
        Vector2 direction = currentCenterPoint - LastFrameCenterPoint;
        float distanceForRay = direction.magnitude;
        float radius = Collider.radius / 2;

        rayHits = Physics2D.CircleCastAll(LastFrameCenterPoint, radius, direction, distanceForRay, ColliderLayerMask);

        if(rayHits.Length > 0)
        {
            rayHits.SortByLength();
            RegisterCollision(rayHits[0]);
        }
    }

    private void OnCollision(BallBase ball, RaycastHit2D rayHit, CollisionSide colSide, CollisionType colType, float endSpeed, int safe, out float distanceAfterHit)
    {
        Vector2 lastFramePos = ball.LastFrameCenterPoint;
        Vector2 velocity = LastFrameVelocity;
        Vector2 actualPos = currentCenterPoint;
        Vector2 centroidPoint = rayHit.centroid;
        Vector2 contactPoint = rayHit.point;

        float distanceToCollision = (lastFramePos - centroidPoint).magnitude;
        float totalDistance = (actualPos - lastFramePos).magnitude;

        distanceAfterHit = totalDistance - distanceToCollision;

        if (distanceToCollision == 0)
        {
            Debug.LogError("FAIL");
        }

        // Debug.LogFormat("[{3}] distanceToCollision: {0}, total distance: {1} distanceAfterHit: {2}", distanceToCollision, totalDistance, distanceAfterHit, safe);

        Debug.DrawLine(centroidPoint, contactPoint, Color.blue, 2);
        Debug.DrawLine(actualPos, lastFramePos, safe == 0 ? Color.black : Color.yellow, 2);
        Debug.DrawLine(actualPos, centroidPoint, Color.green, 2);

        Vector2 newPos;

        if(distanceToCollision == 0)
        {
            newPos = GetNewPositionWhenOverlaping(colType, colSide, rayHit, actualPos, Collider.radius / 2);
        }
        else
        {
            newPos = GetNewPositionBasedOnCollisionType(colType, colSide, centroidPoint, velocity, distanceAfterHit);
        }

        ball.transform.position = newPos;
        Debug.DrawLine(centroidPoint, newPos, safe == 0 ? Color.red : Color.grey, 2);
        Vector2 endVel = SetVelocityBasedOnCollisionType(colType, colSide, endSpeed, LastFrameVelocity);
        currentCenterPoint = newPos;

        if(distanceToCollision == 0)
        {
            rayHits = new RaycastHit2D[0];
        }
        else
        {
            rayHits = Physics2D.CircleCastAll(centroidPoint, Collider.radius / 2, newPos - centroidPoint, distanceAfterHit, ColliderLayerMask);
            rayHits = rayHits.Remove(rayHit);
        }

        LastFrameVelocity = endVel;
        LastFrameCenterPoint = rayHit.centroid;

       // Debug.LogFormat("col Y: {0}, end Y: {1}", centroidPoint.y, newPos.y);
      //  Debug.LogFormat("start vel: {0} end vel: {1}", velocity.normalized, endVel.normalized);
      
    }

    private Vector2 GetNewPositionBasedOnCollisionType(CollisionType colType, CollisionSide colSide, Vector2 centroidPoint, Vector2 velocity, float distanceAfterHit)
    {
        switch (colType)
        {
            case CollisionType.Frame:
            case CollisionType.Enemy:
            case CollisionType.Ship:
                return GetOppositePosition(colSide, centroidPoint, velocity, distanceAfterHit);
            case CollisionType.Striker:
                return GetPositionOnStrikerHit(colSide, centroidPoint, velocity, distanceAfterHit);
            default:
                return Vector2.zero;
        }
    }

    private Vector2 GetOppositePosition(CollisionSide colSide, Vector2 centroidPoint, Vector2 velocity, float distanceAfterHit)
    {
        return centroidPoint + colSide.GetOppositeNormalizedVector(velocity) * distanceAfterHit;
    }

    private Vector2 GetPositionOnStrikerHit(CollisionSide colSide, Vector2 centroidPoint, Vector2 velocity, float distanceAfterHit)
    {
        return centroidPoint + striker.GetForceOnBallHit(this, colSide).normalized * distanceAfterHit;
    }

    private Vector2 SetVelocityBasedOnCollisionType(CollisionType colType, CollisionSide colSide, float endSpeed, Vector2 actualVelocity)
    {
        switch (colType)
        {
            case CollisionType.Frame:
            case CollisionType.Enemy:
                return SetOppositeVelocity(colSide, endSpeed, LastFrameVelocity);
            case CollisionType.Ship:
                return SetUpOrDownVelocity(endSpeed, actualVelocity);
            case CollisionType.Striker:
                Vector2 vel = striker.GetForceOnBallHit(this, colSide);
                Rigidbody.velocity = vel;
                return vel;
            default:
                return Vector2.zero;
        }
    }

    public Vector2 SetOppositeVelocity(CollisionSide colliderSide, float endSpeed, Vector2 actualVelocity)
    {
        float xVel = actualVelocity.x;
        float yVel = actualVelocity.y;
        Vector2 endVel = new Vector2();
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                endVel = new Vector2(xVel, -yVel).normalized * endSpeed;
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                endVel = new Vector2(-xVel, yVel).normalized * endSpeed;
                break;
        }

        Rigidbody.velocity = endVel;

        return endVel;
    }

    public Vector2 SetUpOrDownVelocity(float endSpeed, Vector2 actualVelocity)
    {
        float xVel = LastFrameVelocity.x;
        float yVel = LastFrameVelocity.y;

        Vector2 endVel = new Vector2(xVel, -yVel).normalized * endSpeed;
        Rigidbody.velocity = endVel;

        return endVel;
    }

    public void AddToPosition(Vector2 vec)
    {
        LastFrameCenterPoint += vec;
        transform.position += (Vector3)vec;
        currentCenterPoint += vec;
    }

    private Vector2 GetNewPositionWhenOverlaping(CollisionType colType, CollisionSide colSide, RaycastHit2D rayHit, Vector2 actualPos, float radius)
    {
        switch (colType)
        {
            case CollisionType.Frame:
                return GetOverlapPositionForFrame(colType, colSide, rayHit, actualPos, radius);
            case CollisionType.Ship:
            case CollisionType.Enemy:
                return GetOverlapPositionForEnemyOrShip(colType, colSide, rayHit, actualPos, radius);
            case CollisionType.Striker:
                return actualPos; //GetPositionOnStrikerHit(colSide, centroidPoint, velocity, distanceAfterHit);
            default:
                return actualPos;
        }
    }

    private Vector2 GetOverlapPositionForFrame(CollisionType colType, CollisionSide colSide, RaycastHit2D rayHit, Vector2 actualPos, float radius)
    {
        if (colSide == CollisionSide.Bottom || colSide == CollisionSide.Top)
        {
            actualPos.y = rayHit.collider.bounds.center.y;
        }
        else if (colSide == CollisionSide.Left || colSide == CollisionSide.Right)
        {
            actualPos.x = rayHit.collider.bounds.center.x;
        }
        BoxCollider2D boxCollider = rayHit.collider as BoxCollider2D;
        float fromCenterToBorder = boxCollider.size.x * boxCollider.transform.parent.localScale.x / 2;
        float distToMove = radius + fromCenterToBorder;
        Vector2 newPos = actualPos + colSide.GetOppositeDirectionVector() * distToMove;
        Debug.LogErrorFormat("dist to move on overlap: {0} newPos: {1}, from center to border: {2} ", distToMove, newPos, fromCenterToBorder);
        return newPos;
    }

    private Vector2 GetOverlapPositionForEnemyOrShip(CollisionType colType, CollisionSide colSide, RaycastHit2D rayHit, Vector2 actualPos, float radius)
    {
        BoxCollider2D boxCollider = rayHit.collider as BoxCollider2D;
        float fromCenterToBorder = 0;
        switch (colSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                fromCenterToBorder = boxCollider.size.y * boxCollider.transform.localScale.y / 2;
                actualPos.y = rayHit.collider.bounds.center.y;
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                actualPos.x = rayHit.collider.bounds.center.x;
                fromCenterToBorder = boxCollider.size.x * boxCollider.transform.localScale.x / 2;
                break;
        }
        float distToMove = radius + fromCenterToBorder;
        Vector2 newPos = actualPos + colSide.GetOppositeDirectionVector() * distToMove;
        return newPos;
    }


    #region IRayCollider
    public void Raycast()
    {
        RaycastForColliders();
    }

    public void RegisterObject()
    {
        RayCollidersController.Instance.RegisterRayCollider(this);
    }

    public void Unregister()
    {
        if(RayCollidersController.Instance != null)
        {
            RayCollidersController.Instance.UnregisterRayCollider(this);
        }
    }

    public void OnFixedUpdateTick()
    {
        if (skipOneFrameFromCollisionSystem)
        {
            skipOneFrameFromCollisionSystem = false;
            return;
        }
        LastFrameVelocity = Rigidbody.velocity;
        LastFrameCenterPoint = transform.position;
        //  Debug.LogFormat("set last frame pos: {0} -- {1}", LastFrameCenterPoint.x, LastFrameCenterPoint.y);
        SetHittedState();
        SetTrailBasedOnSpeed();
    }

    public List<IRayCollider> HandleCollision(List<IRayCollider> collidersToSkip)
    {
        if (skipOneFrameFromCollisionSystem)
        {
            return new List<IRayCollider>();
        }
        List<IRayCollider> collidedWith = new List<IRayCollider>();

        Vector2 direction = currentCenterPoint - LastFrameCenterPoint;
        float distanceForRay = direction.magnitude;
        float radius = Collider.radius / 2;

        int safe = 0;

        while (rayHits.Length > 0 && distanceForRay > 0)
        {
            rayHits.SortByLength();
            if (rayHits.Length > 1)
            {
                for (int i = 0; i < rayHits.Length; i++)
                {
                    //Debug.LogWarningFormat("[{0}] point: {1} centroid: {2} length: {3}", i, rayHits[i].point, rayHits[i].centroid, rayHits[i].distance);
                }
            }
            RaycastHit2D rayHit = rayHits[0];
            CollisionSide colSide = CollisionSide.Bottom;
            if (rayHit.collider.CompareTag(GameTags.Frame))
            {
                FrameCollider frameCollider = rayHit.collider.GetComponent<FrameCollider>();
                IRayCollider col = frameCollider as IRayCollider;
                if (!collidersToSkip.Contains(col))
                {
                    if (frameCollider.DestroyBallAndProjectiles)
                    {
                        GameController.Instance.RemoveBallFromPlay(this);
                    }
                    else
                    {
                        colSide = frameCollider.CollisionSide;
                        OnCollision(this, rayHit, colSide, CollisionType.Frame, LastFrameVelocity.magnitude, safe, out distanceForRay);
                    }

                    if (!collidedWith.Contains(col))
                    {
                        collidedWith.Add(col);
                    }
                }
            }
            else if (rayHit.collider.CompareTag(GameTags.Enemy))
            {
                // Debug.DrawLine(currentCenterPoint, LastFrameCenterPoint, Color.green, 2);
                EnemyBase enemy = rayHit.collider.GetComponent<EnemyBase>();
                IRayCollider col = enemy as IRayCollider;

                if (!collidersToSkip.Contains(col))
                {
                    colSide = CollisionSideDetect.GetCollisionSide(rayHit.centroid, rayHit.point);
                    OnCollision(this, rayHit, colSide, CollisionType.Enemy, PhysicsConstants.BallSpeedAfterEnemyHit, safe, out distanceForRay);
                    enemy.OnCollisionWithBall(this);
                    if (!collidedWith.Contains(col))
                    {
                        collidedWith.Add(col);
                    }
                }
            }
            else if (rayHit.collider.CompareTag(GameTags.Ship))
            {
                ShipCollider ship = rayHit.collider.GetComponent<ShipCollider>();
                IRayCollider col = ship as IRayCollider;
                if (!collidersToSkip.Contains(col))
                {
                    colSide = ship.GetCollisionSideWithBall(this, LastFrameCenterPoint);
                    OnCollision(this, rayHit, colSide, CollisionType.Ship, PhysicsConstants.BallSpeedAfterShipHit, safe, out distanceForRay);
                }

                if (!collidedWith.Contains(col))
                {
                    collidedWith.Add(col);
                }
            }
            else if (rayHit.collider.CompareTag(GameTags.Striker))
            {
                Debug.Log("col with striker");
                striker = rayHit.collider.GetComponent<Striker>();
                IRayCollider col = striker as IRayCollider;
                if (!collidersToSkip.Contains(col))
                {
                    colSide = striker.GetCollisionSideWithBall(this, LastFrameCenterPoint);
                    OnCollision(this, rayHit, colSide, CollisionType.Striker, striker.GetForceOnBallHit(this, colSide).magnitude, safe, out distanceForRay);
                }

                if (!collidedWith.Contains(col))
                {
                    collidedWith.Add(col);
                }
            }

            //Debug.LogFormat("{0} collision with {1} on side {2}", safe, rayHit.collider.gameObject.name, colSide);

            safe++;
        }

        return collidedWith;
    }

    public void RegisterCollision(RaycastHit2D rayHit)
    {
        RayCollidersController.Instance.RegisterCollision(this, rayHit);
    }
    #endregion
}