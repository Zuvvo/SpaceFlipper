using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour, IRayCollider
{
    public PlayerShip Ship;
    public ShipCollisionDetector ShipCollisionDetector;
    public BoxCollider2D Collider;
    public Transform BottomPoint;
    public LayerMask ColliderLayerMask;

    public Vector2 LastFrameBottomPoint { get; private set; }
    public Vector2 LastFrameVelocity { get; private set; }
    public Vector2 LastFrameCenterPoint { get; private set; }

    private Vector2 currentPosition;

    private RaycastHit2D[] rayHits;

    private void Start()
    {
        RegisterObject();
    }

    private void OnDestroy()
    {
        Unregister();
    }

    public void OnCollision()
    {
        Ship.OnCollisionWithProjectile();
    }

    public CollisionSide GetCollisionSideWithBall(BallBase ball, Vector2 ballLastFramePosition)
    {
        CollisionSide colSide = CollisionSide.Top;

        Vector2 centerPoint = Collider.bounds.center;
        Vector2 targetPoint = ballLastFramePosition;
        float objectCenterToTargetLength = (targetPoint - centerPoint).magnitude;
        float bottomToTargetLength = (targetPoint - LastFrameBottomPoint).magnitude;

        if (objectCenterToTargetLength < bottomToTargetLength)
        {
            colSide = CollisionSide.Bottom;
        }
        else
        {
            colSide = CollisionSide.Top;
        }

        return colSide;
    }
    #region IRayCollider
    public void Raycast()
    {
        currentPosition = Ship.Rigidbody.transform.position;
        Vector2 direction = currentPosition - LastFrameCenterPoint;
        Vector2 boxSize = new Vector2(Collider.size.x * transform.localScale.x, Collider.size.y * transform.localScale.y);
        float distanceForRay = direction.magnitude;

        ShipCollisionDetector.RaycastForCollisions();

        rayHits = ColliderRaycaster.BoxCastAll(LastFrameCenterPoint, boxSize, 0, direction, distanceForRay, ColliderLayerMask);

        if (rayHits.Length > 0)
        {
            rayHits.SortByLength();
            RegisterCollision(rayHits[0]);
        }
    }

    public void RegisterObject()
    {
        LastFrameVelocity = Ship.Rigidbody.velocity;
        RayCollidersController.Instance.RegisterRayCollider(this, addAsFirst: true);
    }

    public void Unregister()
    {
        if(RayCollidersController.Instance != null)
        {
            RayCollidersController.Instance.UnregisterRayCollider(this);
        }
    }

    public void OnUpdate()
    {
        Ship.ShipController.OnUpdate();
        LastFrameBottomPoint = BottomPoint.position;
        LastFrameVelocity = Ship.Rigidbody.velocity;
        LastFrameCenterPoint = Ship.Rigidbody.transform.position;
    }

    public List<IRayCollider> RayCollision(List<IRayCollider> collidersToSkip)
    {
        List<IRayCollider> collidedWith = new List<IRayCollider>();
        Vector2 direction = currentPosition - LastFrameCenterPoint;
        float distanceForRay = direction.magnitude;

        RaycastHit2D rayHit = rayHits[0];
        CollisionSide colSide = CollisionSide.Bottom;
        if (rayHit.collider.CompareTag(GameTags.Ball))
        {
            BallBase ballBase = rayHit.collider.GetComponent<BallBase>();

            colSide = CollisionSideDetect.GetCollisionSide(rayHit.centroid, rayHit.point);
            OnCollisionWithBall(ballBase, rayHit, colSide, direction, out distanceForRay);


            Vector2 newPos = rayHit.centroid;
            Ship.Rigidbody.transform.position = newPos;
            Debug.DrawLine(newPos, LastFrameCenterPoint, Color.cyan, 4);
        }
        else if (rayHit.collider.CompareTag(GameTags.Ship))
        {
            ShipCollider ship = rayHit.collider.GetComponent<ShipCollider>();
            Debug.LogWarningFormat("ray for another ship hit");
            //colSide = ship.GetCollisionSideWithBall(this, LastFrameCenterPoint);
            // OnCollision(this, rayHit, colSide, CollisionType.Ship, PhysicsConstants.BallSpeedAfterShipHit, safe, out distanceForRay);
        }

        Debug.LogWarningFormat("Ship collision with {0} on side {1}", rayHit.collider.gameObject.name, colSide);
        return collidedWith;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            CollisionSide colSide = CollisionSideDetect.GetCollisionSide(LastFrameCenterPoint, collision.GetContact(0).point);
            Debug.Log("[SHIPCOLLIDER} colliding with ball " + colSide);
        }
    }

    private void OnCollisionWithBall(BallBase ball, RaycastHit2D rayHit, CollisionSide colSide, Vector2 direction, out float distanceForRay)
    {
        Vector2 lastFramePos = ball.LastFrameCenterPoint;
        Vector2 velocity = LastFrameVelocity;
        Vector2 actualPos = currentPosition;
        Vector2 centroidPoint = rayHit.centroid;
        Vector2 contactPoint = rayHit.point;

        float distanceToCollision = (lastFramePos - centroidPoint).magnitude;
        float totalDistance = (actualPos - lastFramePos).magnitude;
        float distanceAfterHit = totalDistance - distanceToCollision;

        distanceForRay = distanceAfterHit;

        Vector2 distanceToAdd = distanceAfterHit * direction;

        ball.AddToPosition(distanceToAdd);
    }

    public void RegisterCollision(RaycastHit2D rayHit)
    {
        Debug.LogWarningFormat("register collision here");
        RayCollidersController.Instance.RegisterCollision(this, rayHit);
    }
    #endregion
}