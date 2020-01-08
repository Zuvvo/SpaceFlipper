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

    public Rigidbody2D Rigidbody;

    public void Init(PlayerInfo playerInfo)
    {
        PlayerInfo = playerInfo;
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