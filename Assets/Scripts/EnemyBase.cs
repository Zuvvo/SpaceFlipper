using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public List<EnemyShooter> Shooters = new List<EnemyShooter>();

    private EnemySpawner associatedSpawner;

    private void OnDestroy()
    {
        EnemyController enemyController = EnemyController.Instance;
        if(enemyController != null)
        {
            EnemyController.Instance.UnregisterEnemy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.collider.GetComponent<BallBase>();
            if(ball != null)
            {
                ContactPoint contactPoint = collision.GetContact(0);
                CollisionSide colSide = CollisionSideDetect.GetCollisionSide(ball.LastFrameCenterPoint, contactPoint.point);
                ball.SetOppositeVelocity(colSide, PhysicsConstants.BallSpeedAfterEnemyHit);
            }
            associatedSpawner.TryToDestroy(this);
        }
    }

    public void Init(EnemySpawner enemySpawner)
    {
        associatedSpawner = enemySpawner;
    }

    public void Shoot()
    {
        if(Shooters.Count > 0)
        {
            Shooters[UnityEngine.Random.Range(0, Shooters.Count)].InitShot();
        }
    }
}
