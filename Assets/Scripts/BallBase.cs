using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBase : MonoBehaviour
{
    public Rigidbody RigidBody;
    public void Stop()
    {
        RigidBody.velocity = Vector3.zero;
        RigidBody.angularVelocity = Vector3.zero;
    }

    private void Start()
    {
        RigidBody.AddForce(new Vector3(-1, 0, 0), ForceMode.Impulse);
    }
}