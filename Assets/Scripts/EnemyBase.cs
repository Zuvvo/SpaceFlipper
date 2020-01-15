using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public Color OneDamageColor;
    public Color TwoDamageColor;
    public Color ThreeDamageColor;

    public List<EnemyShooter> Shooters = new List<EnemyShooter>();

    private float maxHealth = 4000;

    private EnemySpawner associatedSpawner;
    private float shotDelayMin = 2f;
    private float shotDelayMax = 3.5f;

    private float currentHealth;

    private void OnDestroy()
    {
        EnemyController enemyController = EnemyController.Instance;
        if(enemyController != null)
        {
            EnemyController.Instance.UnregisterEnemy(this);
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(EnemyShotRoutine());
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag(GameTags.Ball))
    //    {
    //        BallBase ball = collision.collider.GetComponent<BallBase>();
    //        if(ball != null)
    //        {
    //            ContactPoint2D contactPoint = collision.GetContact(0);
    //            CollisionSide colSide = CollisionSideDetect.GetCollisionSide(ball.LastFrameCenterPoint, contactPoint.point);
    //            ball.SetOppositeVelocity(colSide, PhysicsConstants.BallSpeedAfterEnemyHit);
    //            if (ball.LastFrameVelocity.magnitude < PhysicsConstants.BallSpeedPowerShotThreshold)
    //            {
    //                currentHealth--;
    //            }
    //            else
    //            {
    //                currentHealth -= 3;
    //            }
    //            SetColor();
    //        }
    //        if(currentHealth <= 0)
    //        {
    //            associatedSpawner.TryToDestroy(this);
    //        }
    //    }
    //}

    public void OnCollisionWithBall(BallBase ball)
    {
        if (ball.LastFrameVelocity.magnitude < PhysicsConstants.BallSpeedPowerShotThreshold)
        {
            currentHealth--;
        }
        else
        {
            currentHealth -= 3;
        }

        if (currentHealth <= 0)
        {
            associatedSpawner.TryToDestroy(this);
        }
    }

    private void SetColor()
    {
        switch (currentHealth)
        {
            case 3:
                SpriteRenderer.color = OneDamageColor;
                break;
            case 2:
                SpriteRenderer.color = TwoDamageColor;
                break;
            case 1:
                SpriteRenderer.color = ThreeDamageColor;
                break;
        }
    }

    public void Init(EnemySpawner enemySpawner)
    {
        associatedSpawner = enemySpawner;
    }

    private IEnumerator EnemyShotRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(shotDelayMin, shotDelayMax));
            Shoot();
        }
    }

    public void Shoot()
    {
        if(Shooters.Count > 0)
        {
            Shooters[UnityEngine.Random.Range(0, Shooters.Count)].InitShot();
        }
    }
}
