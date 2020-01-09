using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameTrigger : MonoBehaviour
{
    public CollisionSide CollisionSide;
    public bool DestroyBallAndProjectiles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameTags.Ball))
        {
            Debug.Log("OnTriggerEnter2D");
            BallBase ball = collision.gameObject.GetComponent<BallBase>();
            
            if (ball != null)
            {
                if (DestroyBallAndProjectiles)
                {
                    GameController.Instance.RemoveBallFromPlay(ball);
                }
                else
                {
                    ball.SetOppositeVelocity(CollisionSide);
                }
            }
        }
    }
}
