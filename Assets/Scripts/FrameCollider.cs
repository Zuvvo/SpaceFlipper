using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.collider.GetComponent<BallBase>();
            if (ball != null)
            {
                ContactPoint2D contactPoint = collision.GetContact(0);
                CollisionSide colSide = CollisionSideDetect.GetCollisionSide(ball.LastFrameCenterPoint, contactPoint.point);
                ball.SetOppositeVelocity(colSide);
            }
        }
    }
}
