using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Projectile ProjectilePrefab;
    public Transform ProjectileSpawnPlace;

    private Projectile _lastProjectile;

    private float minForce = -0.4f;
    private float maxForce = -0.6f;

    private float minAngle = -0.05f;
    private float maxAngle = 0.05f;

    public void InitShot()
    {
        _lastProjectile = Instantiate(ProjectilePrefab, ProjectileSpawnPlace.position, Quaternion.identity);
        _lastProjectile.Rigidbody.AddForce(new Vector3(UnityEngine.Random.Range(minForce, maxForce), 0, UnityEngine.Random.Range(minAngle, maxAngle)), ForceMode.Impulse);
    }
}