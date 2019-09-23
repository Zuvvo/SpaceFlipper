using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody Rigidbody;

    private Vector3 lastFramePos;

    private void Start()
    {
        lastFramePos = transform.position;
        Destroy(gameObject, 8);
    }

    private void Update()
    {
        RaycastForShipToLastFramePosition();
        lastFramePos = transform.position;
    }

    private void RaycastForShipToLastFramePosition()
    {
        RaycastHit ray;
        Debug.DrawLine(transform.position, lastFramePos, Color.red, 0.1f);
        if (Physics.Raycast(transform.position, transform.position - lastFramePos, out ray, (transform.position - lastFramePos).magnitude))
        {
            ShipCollider shipCollider = ray.collider.GetComponent<ShipCollider>();
            OnCollisionWithShip(shipCollider);
        }
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
}