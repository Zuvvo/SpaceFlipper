using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTrigger : MonoBehaviour
{
    public PlayerShip Ship;
    public Transform BottomPoint;
    public BoxCollider2D Collider;

    public Vector2 LastFrameBottomPoint { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.gameObject.GetComponent<BallBase>();
            if (ball != null)
            {
                AddForceBasedOnHitStrikerState(ball);
            }
        }
    }



    private void Update()
    {
        LastFrameBottomPoint = BottomPoint.position;
    }

    private void AddForceBasedOnHitStrikerState(BallBase ball)
    {
        CollisionSide colSide = CollisionSide.Top;

        Vector2 centerPoint = Collider.bounds.center;
        Vector2 targetPoint = ball.LastFrameCenterPoint;
        float objectCenterToTargetLength = (targetPoint - centerPoint).magnitude;
        float bottomToTargetLength = (targetPoint - LastFrameBottomPoint).magnitude;

        if (objectCenterToTargetLength < bottomToTargetLength)
        {
            colSide = CollisionSide.Top;
        }
        else
        {
            colSide = CollisionSide.Bottom;
        }

        ball.AddForceOnShipHit(colSide);
    }
}
