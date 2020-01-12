using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Projectile ProjectilePrefab;
    public Transform ProjectileSpawnPlace;

    private Projectile _lastProjectile;

    private float minForce = -2f;
    private float maxForce = -3f;

    private float minAngle = -0.25f;
    private float maxAngle = 0.25f;

    public void InitShot()
    {
        _lastProjectile = Instantiate(ProjectilePrefab, ProjectileSpawnPlace.position, Quaternion.identity);
        Vector2 forceVector = new Vector2(Random.Range(minAngle, maxAngle), Random.Range(minForce, maxForce));
        _lastProjectile.Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }
}