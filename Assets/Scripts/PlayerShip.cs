using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShip : MonoBehaviour
{
    public float Health = 3;
    public ShipController ShipController;
    public PlayerInfo PlayerInfo { get; private set; }

    public Rigidbody RigidBody;

    public void Init(PlayerInfo playerInfo)
    {
        PlayerInfo = playerInfo;
    }

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
        GameController.Instance.CallOnGameStateChanged();
        Debug.LogError("Health: " + Health);
        if(Health == 0)
        {
            GameController.Instance.EndGameLose();
        }
    }
}