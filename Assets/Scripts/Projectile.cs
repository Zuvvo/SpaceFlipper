using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D Rigidbody;

    private Vector3 lastFramePos;
    private RaycastHit2D rayHit;

    private void Start()
    {
        lastFramePos = transform.position;
    }

    private void Update()
    {
        if (!RaycastForShipToLastFramePosition())
        {
            RaycastForLevelFrame();
            lastFramePos = transform.position;
        }
    }

    private void RaycastForLevelFrame()
    {
        rayHit = Physics2D.Raycast(transform.position, transform.position - lastFramePos, (transform.position - lastFramePos).magnitude, LayerConstants.Frame);
        if (rayHit.collider != null)
        {
            DestroyProjectile();
        }
    }

    private bool RaycastForShipToLastFramePosition()
    {
        Debug.DrawLine(transform.position, lastFramePos, Color.red, 0.1f);
        rayHit = Physics2D.Raycast(transform.position, transform.position - lastFramePos, (transform.position - lastFramePos).magnitude, LayerConstants.Ship);
        if (rayHit.collider != null)
        {
            ShipCollider shipCollider = rayHit.collider.GetComponent<ShipCollider>();
            OnCollisionWithShip(shipCollider);
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameTags.Ship))
        {
            ShipCollider shipCollider = other.GetComponent<ShipCollider>();
            OnCollisionWithShip(shipCollider);
        }
    }

    private void OnCollisionWithShip(ShipCollider shipCollider)
    {
        if (shipCollider != null)
        {
            Destroy(gameObject);
            shipCollider.OnCollision();
        }
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}