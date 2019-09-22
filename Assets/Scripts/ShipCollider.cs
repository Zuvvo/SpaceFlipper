using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour
{
    public Ship Ship;

    public void OnCollision()
    {
        Ship.OnCollisionWithProjectile();
    }
}