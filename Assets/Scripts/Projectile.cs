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
    }

    private void Update()
    {
        RaycastForObjectOnLastFramePosition();
        lastFramePos = transform.position;
    }

    private void RaycastForObjectOnLastFramePosition()
    {
        RaycastHit ray;
        Debug.DrawLine(transform.position, lastFramePos, Color.red, 0.1f);
        if (Physics.Raycast(transform.position, transform.position - lastFramePos, out ray, (transform.position - lastFramePos).magnitude))
        {
            ShipCollider shipCollider = ray.collider.GetComponent<ShipCollider>();
            if (shipCollider != null)
            {
                Debug.Log("hit");
                Destroy(gameObject);
                shipCollider.OnCollision();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameTags.Ship))
        {
            ShipCollider shipCollider = other.GetComponent<ShipCollider>();
            if (shipCollider != null)
            {
                Destroy(gameObject);
                shipCollider.OnCollision();
            }
        }
    }
}