using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour
{
    public PlayerShip Ship;
    public BoxCollider2D Collider;
    public Transform BottomPoint;

    public Vector2 LastFrameBottomPoint { get; private set; }

    private void Update()
    {
        LastFrameBottomPoint = BottomPoint.position;
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
}