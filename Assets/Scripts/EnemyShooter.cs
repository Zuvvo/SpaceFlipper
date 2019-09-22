using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Projectile ProjectilePrefab;
    public Transform ProjectileSpawnPlace;

    private Projectile _lastProjectile;

    public void InitShot()
    {
        _lastProjectile = Instantiate(ProjectilePrefab, ProjectileSpawnPlace.position, Quaternion.identity);
        _lastProjectile.Rigidbody.AddForce(new Vector3(-0.6f, 0, 0), ForceMode.Impulse);
    }
}