using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCollider : MonoBehaviour, ICollider
{
    public CollisionSide CollisionSide;
    public bool DestroyBallAndProjectiles;

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag(GameTags.Ball))
    //    {
    //        BallBase ball = collision.gameObject.GetComponent<BallBase>();

    //        if (ball != null)
    //        {
    //            Debug.Log("OnCollisionEnter2D");
    //            if (DestroyBallAndProjectiles)
    //            {
    //                GameController.Instance.RemoveBallFromPlay(ball);
    //            }
    //            else
    //            {
    //                ball.SetOppositeVelocity(CollisionSide);
    //            }
    //        }
    //    }
    //}
}
