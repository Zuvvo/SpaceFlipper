using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemySpawner associatedSpawner;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.collider.GetComponent<BallBase>();
            if(ball != null)
            {
                ContactPoint contactPoint = collision.GetContact(0);
                CollisionSide colSide = CollisionSideDetect.GetCollisionSide(ball.LastFrameCenterPoint, contactPoint.point);
                ball.SetOppositeVelocity(colSide);
            }
            associatedSpawner.TryToDestroy(this);
        }
    }

    public void Init(EnemySpawner enemySpawner)
    {
        associatedSpawner = enemySpawner;
    }
}
