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

    private float distToMoveOnOverlap = 0.02f;
    private float rayDistOnOverlap = 0.05f;
    private float defaultToTotalDistance { get { return (currentCenterPoint - LastFrameCenterPoint).magnitude; } }

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

    private void OnCollision(BallBase ball, RaycastHit2D rayHit, CollisionSide colSide, CollisionType colType, float totalDistance, float endSpeed, int safe, ICollider collidingObject, out float distanceAfterHit)
    {
        if(totalDistance <= 0)
        {
            rayHits = new RaycastHit2D[0];
            Debug.LogWarning("TOTAL DISTNANCE <= 0!");
            distanceAfterHit = 0;
            return;
        }

        Vector2 lastFramePos = ball.LastFrameCenterPoint;
        Vector2 velocity = LastFrameVelocity;
        Vector2 actualPos = currentCenterPoint;
        Vector2 centroidPoint = rayHit.centroid;

        float colRadius = Collider.radius / 2;
        float distanceToCollision = rayHit.distance;

        distanceAfterHit = totalDistance - distanceToCollision;
        // Debug.LogFormat("[{3}] distanceToCollision: {0}, total distance: {1} distanceAfterHit: {2}", distanceToCollision, totalDistance, distanceAfterHit, safe);

        Debug.DrawLine(centroidPoint, rayHit.point, Color.blue, 2);
        Debug.DrawLine(actualPos, lastFramePos, safe == 0 ? Color.black : Color.yellow, 2);
        Debug.DrawLine(actualPos, centroidPoint, Color.green, 2);

        Vector2 newPos;
        bool overlapping = distanceToCollision == 0;
        Debug.LogFormat("Collision. side: {0} type: {1} overlap: {2}, last framePos: {3} actualPos: {4}", colSide, colType, overlapping, lastFramePos, actualPos);
        if (overlapping)
        {
            Debug.LogWarningFormat("OVERLAPPING! To collision: {0} total distance: {1}", distanceToCollision, totalDistance);
            overlapping = true;
            newPos = GetNewPositionWhenOverlaping(colType, colSide, collidingObject, rayHit, actualPos, colRadius);
            distanceAfterHit = totalDistance - (newPos - lastFramePos).magnitude;
            Debug.Log("new distance after hit: " + distanceAfterHit);
            if(distanceAfterHit <= -100)
            {
                Debug.Log("FAIL");
            }
           // newPos = GetNewPositionBasedOnCollisionType(colType, colSide, newPos, velocity, distanceAfterHit);
           // Debug.LogFormat("Pos changed: {0} to {1}", lastFramePos, newPos);
        }
        else
        {
            newPos = GetNewPositionBasedOnCollisionType(colType, colSide, centroidPoint, velocity, collidingObject, distanceAfterHit);

            Vector2 endVel = SetVelocityBasedOnCollisionType(colType, colSide, collidingObject, endSpeed, LastFrameVelocity);
            LastFrameVelocity = endVel;
            Debug.Log("change velocity to " + (endVel.y < 0 ? "DOWN" : "UP"));
        }

        ball.transform.position = newPos;
        Debug.DrawLine(centroidPoint, newPos, safe == 0 ? Color.red : Color.grey, 2);
        currentCenterPoint = newPos;

        if (overlapping)
        {
            rayHit.centroid = newPos;
            rayHit.distance = distToMoveOnOverlap;
            //rayHits = new RaycastHit2D[1] { rayHit };
            //rayHits = Physics2D.CircleCastAll(newPos, colRadius, newPos - actualPos, distanceAfterHit, ColliderLayerMask);
            OnCollision(ball, rayHit, colSide, colType, distanceAfterHit, endSpeed, safe, collidingObject, out distanceAfterHit);
            LastFrameCenterPoint = newPos;
        }
        else
        {
            rayHits = Physics2D.CircleCastAll(centroidPoint, colRadius, newPos - centroidPoint, distanceAfterHit, ColliderLayerMask);
            LastFrameCenterPoint = rayHit.centroid;
            rayHits = rayHits.Remove(rayHit);
        }
        

       // Debug.LogFormat("col Y: {0}, end Y: {1}", centroidPoint.y, newPos.y);
      //  Debug.LogFormat("start vel: {0} end vel: {1}", velocity.normalized, endVel.normalized);
      
    }

    private Vector2 GetNewPositionBasedOnCollisionType(CollisionType colType, CollisionSide colSide, Vector2 centroidPoint, Vector2 velocity, ICollider collidingObject, float distanceAfterHit)
    {
        if(distanceAfterHit <= 0)
        {
            return centroidPoint;
        }

        switch (colType)
        {
            case CollisionType.Frame:
            case CollisionType.Enemy:
            case CollisionType.Ship:
                return GetOppositePosition(colSide, centroidPoint, velocity, distanceAfterHit);
            case CollisionType.StrikerLeft:
            case CollisionType.StrikerRight:
                return GetPositionOnStrikerHit(colSide, collidingObject, centroidPoint, velocity, distanceAfterHit);
            default:
                return Vector2.zero;
        }
    }

    private Vector2 GetOppositePosition(CollisionSide colSide, Vector2 centroidPoint, Vector2 velocity, float distanceAfterHit)
    {
        return centroidPoint + colSide.GetCollisionDirectionVector(velocity) * distanceAfterHit;
    }

    private Vector2 GetPositionOnStrikerHit(CollisionSide colSide, ICollider collidingObject, Vector2 centroidPoint, Vector2 velocity, float distanceAfterHit)
    {
        Striker striker = collidingObject as Striker;
        return centroidPoint + striker.GetForceOnBallHit(this, colSide).normalized * distanceAfterHit;
    }

    private Vector2 SetVelocityBasedOnCollisionType(CollisionType colType, CollisionSide colSide, ICollider collidingObject, float endSpeed, Vector2 actualVelocity)
    {
        switch (colType)
        {
            case CollisionType.Frame:
            case CollisionType.Enemy:
                return SetVelocity(colSide, endSpeed, LastFrameVelocity);
            case CollisionType.Ship:
                return SetVelocity(colSide, endSpeed, actualVelocity);
            case CollisionType.StrikerLeft:
            case CollisionType.StrikerRight:
                Striker striker = collidingObject as Striker;
                Vector2 vel = striker.GetForceOnBallHit(this, colSide);
                Rigidbody.velocity = vel;
                return vel;
            default:
                return Vector2.zero;
        }
    }

    public Vector2 SetVelocity(CollisionSide colliderSide, float endSpeed, Vector2 actualVelocity)
    {
        float xVel = actualVelocity.x;
        float yVel = actualVelocity.y;

        float xVelAbs = Mathf.Abs(xVel);
        float yVelAbs = Mathf.Abs(yVel);

        Vector2 endVel = new Vector2();
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
                endVel = new Vector2(xVel, yVelAbs).normalized * endSpeed;
                break;
            case CollisionSide.Top:
                endVel = new Vector2(xVel, -yVelAbs).normalized * endSpeed;
                break;
            case CollisionSide.Left:
                endVel = new Vector2(xVelAbs, yVel).normalized * endSpeed;
                break;
            case CollisionSide.Right:
                endVel = new Vector2(-xVelAbs, yVel).normalized * endSpeed;
                break;
        }

        Rigidbody.velocity = endVel;
        return endVel;
    }

    /*
    public Vector2 SetUpOrDownVelocity(CollisionSide colSide, float endSpeed, Vector2 actualVelocity)
    {
        float xVel = LastFrameVelocity.x;
        float yVel = LastFrameVelocity.y;

        Vector2 endVel = new Vector2(xVel, -yVel).normalized * endSpeed;
        Rigidbody.velocity = endVel;

        return endVel;
    }*/

    public void AddToPosition(Vector2 vec)
    {
        LastFrameCenterPoint += vec;
        transform.position += (Vector3)vec;
        currentCenterPoint += vec;
    }

    private Vector2 GetNewPositionWhenOverlaping(CollisionType colType, CollisionSide colSide, ICollider collidingObject, RaycastHit2D rayHit, Vector2 actualPos, float radius)
    {
        switch (colType)
        {
            case CollisionType.Frame:
                return GetOverlapPositionForFrame(colSide, rayHit, actualPos, radius);
            case CollisionType.Ship:
            case CollisionType.Enemy:
                return GetOverlapPositionForEnemyOrShip(colSide, rayHit, actualPos, radius);
            case CollisionType.StrikerLeft:
            case CollisionType.StrikerRight:
                Striker striker = collidingObject as Striker;
                return GetOverlapPositionForStriker(striker, colSide, rayHit, actualPos, radius);
            default:
                return actualPos;
        }
    }

    private Vector2 GetOverlapPositionForStriker(Striker striker, CollisionSide colSide, RaycastHit2D rayHit, Vector2 actualPos, float radius)
    {
        Vector2 startPos = actualPos;
        Vector2 newDirectionVector = new Vector2();
        switch (colSide)
        {
            case CollisionSide.Bottom:
                newDirectionVector = striker.transform.up;
                break;
            case CollisionSide.Top:
                newDirectionVector = striker.transform.up * -1;
                break;
            case CollisionSide.Left:
                newDirectionVector = striker.transform.right;
                break;
            case CollisionSide.Right:
                newDirectionVector = striker.transform.right * -1;
                break;
        }

        BoxCollider2D boxCollider = rayHit.collider as BoxCollider2D;
        float fromCenterToBorderHorizontally = boxCollider.size.x * boxCollider.transform.localScale.x / 2;
        float fromCenterToBorderVertically = boxCollider.size.y * boxCollider.transform.localScale.y / 2;

        Vector2 a = (Vector2)boxCollider.bounds.center + fromCenterToBorderHorizontally * (Vector2)striker.transform.right * -1;
        Vector2 b = (Vector2)boxCollider.bounds.center + fromCenterToBorderHorizontally * (Vector2)striker.transform.right;
        Vector2 c = (Vector2)boxCollider.bounds.center + fromCenterToBorderVertically * (Vector2)striker.transform.up;
        Vector2 d = (Vector2)boxCollider.bounds.center + fromCenterToBorderVertically * (Vector2)striker.transform.up * -1;
        Vector2 e = new Vector2();
        Vector2 f = new Vector2();
        float distToMove = 0;
        switch (colSide)
        {
            case CollisionSide.Bottom:
            case CollisionSide.Top:
                e = actualPos + newDirectionVector * 10;
                f = actualPos - newDirectionVector * 10;
                actualPos =  Math2D.GetIntersectionPointCoordinates(a, b, e, f);
                distToMove = radius + fromCenterToBorderVertically + distToMoveOnOverlap;
                break;
            case CollisionSide.Left:
            case CollisionSide.Right:
                e = actualPos + newDirectionVector * 10;
                f = actualPos - newDirectionVector * 10;
                actualPos = Math2D.GetIntersectionPointCoordinates(c, d, e, f);
                distToMove = radius + fromCenterToBorderHorizontally + distToMoveOnOverlap;
                break;
        }
        Vector2 newPos = actualPos + newDirectionVector * distToMove;
        Debug.DrawLine(startPos, newPos, Color.white, 4);
        return newPos;
    }

    private Vector2 GetOverlapPositionForFrame(CollisionSide colSide, RaycastHit2D rayHit, Vector2 actualPos, float radius)
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
        float distToMove = radius + fromCenterToBorder + distToMoveOnOverlap;
        Vector2 newPos = actualPos + colSide.GetOppositeDirectionVector() * distToMove;
        Debug.LogWarningFormat("dist to move on overlap: {0} newPos: {1}, from center to border: {2} ", distToMove, newPos, fromCenterToBorder);
        return newPos;
    } 

    private Vector2 GetOverlapPositionForEnemyOrShip(CollisionSide colSide, RaycastHit2D rayHit, Vector2 actualPos, float radius)
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
        float distToMove = radius + fromCenterToBorder + distToMoveOnOverlap;
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
                        OnCollision(this, rayHit, colSide, CollisionType.Frame, defaultToTotalDistance, LastFrameVelocity.magnitude, safe, frameCollider, out distanceForRay);
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
                    OnCollision(this, rayHit, colSide, CollisionType.Enemy, defaultToTotalDistance, PhysicsConstants.BallSpeedAfterEnemyHit, safe, enemy, out distanceForRay);
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
                    OnCollision(this, rayHit, colSide, CollisionType.Ship, defaultToTotalDistance, PhysicsConstants.BallSpeedAfterShipHit, safe, ship, out distanceForRay);
                }

                if (!collidedWith.Contains(col))
                {
                    collidedWith.Add(col);
                }
            }
            else if (rayHit.collider.CompareTag(GameTags.Striker))
            {
                Striker striker = rayHit.collider.GetComponent<Striker>();
                IRayCollider col = striker as IRayCollider;
                if (!collidersToSkip.Contains(col))
                {
                    colSide = striker.GetCollisionSideWithBall(LastFrameCenterPoint);
                    OnCollision(this, rayHit, colSide, striker.CollisionType, defaultToTotalDistance, striker.GetForceOnBallHit(this, colSide).magnitude, safe, striker, out distanceForRay);
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