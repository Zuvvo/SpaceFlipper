using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBase : MonoBehaviour
{
    public Rigidbody RigidBody;

    private float minVel = 0.1f;
    private float maxVel = 0.5f;

    private void Start()
    {
        RigidBody.AddForce(new Vector3(-1, 0, 0), ForceMode.Impulse);
    }

    public void Stop()
    {
        RigidBody.velocity = Vector3.zero;
        RigidBody.angularVelocity = Vector3.zero;
    }


    public void SetOppositeVelocity(CollisionSide colliderSide)
    {
        Vector3 currentVelocity = RigidBody.velocity;
        float clampedXVelocity = Mathf.Clamp(currentVelocity.x, -maxVel, maxVel);
        float clampedZVelocity = Mathf.Clamp(currentVelocity.z, -maxVel, maxVel);
        switch (colliderSide)
        {
            case CollisionSide.Bottom:
                float xBotVel = currentVelocity.x;
                RigidBody.velocity = new Vector3(Mathf.Clamp(-xBotVel, -maxVel, -minVel), currentVelocity.y, clampedZVelocity);
                break;
            case CollisionSide.Top:
                float xTopVel = currentVelocity.x;
                RigidBody.velocity = new Vector3(Mathf.Clamp(-xTopVel, minVel, maxVel), currentVelocity.y, clampedZVelocity);
                break;
            case CollisionSide.Left:
                float zLeftVel = currentVelocity.z;
                RigidBody.velocity = new Vector3(clampedXVelocity, currentVelocity.y, Mathf.Clamp(-zLeftVel, minVel, maxVel));
                break;
            case CollisionSide.Right:
                float zRightVel = currentVelocity.z;
                RigidBody.velocity = new Vector3(clampedXVelocity, currentVelocity.y, Mathf.Clamp(-zRightVel, -maxVel, -minVel));
                break;

        }
    }
}