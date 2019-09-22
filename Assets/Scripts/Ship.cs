using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ship : MonoBehaviour
{
    public float Health = 3;

    public Rigidbody RigidBody;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.collider.GetComponent<BallBase>();
            if (ball != null)
            {
                ball.AddForceOnShipHit();
            }
        }
    }

    public void OnCollisionWithProjectile()
    {
        Health--;
        Debug.LogError("Health: " + Health);
        if(Health == 0)
        {
            SceneManager.LoadScene("Game");
        }
    }
}