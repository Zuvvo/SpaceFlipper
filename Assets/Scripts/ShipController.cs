using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShipController : MonoBehaviour
{
    public KeyCode DodgeKey = KeyCode.LeftShift;

    public StrikerController StrikerController;
    public Ship Ship;
    public float Speed = 20;
    public float DodgeSpeed = 100;
    public float DodgeTimeDelay = 2;
    public float DodgeMoveTime = 0.1f;
    private bool isDodgeReady = true;
    private bool isDodgeMoving;

    private void Update()
    {
        float dirHorizontal = Input.GetAxisRaw("Horizontal");
        float dirVertical = Input.GetAxisRaw("Vertical");
        Ship.RigidBody.velocity = new Vector3(dirVertical * (Speed / 2.5f), 0, -dirHorizontal * (isDodgeMoving ? DodgeSpeed : Speed));

        if (isDodgeReady && Input.GetKeyDown(DodgeKey) && dirHorizontal != 0)
        {
            TryDodge();
        }
    }

    private void TryDodge()
    {
        StartCoroutine(DodgeSequence());
    }

    private IEnumerator DodgeSequence()
    {
        StrikerController.OnDodge();
        isDodgeMoving = true;
        isDodgeReady = false;
        yield return new WaitForSeconds(DodgeMoveTime);
        isDodgeMoving = false;
        yield return new WaitForSeconds(DodgeTimeDelay - DodgeMoveTime);
        isDodgeReady = true;
        StrikerController.SetStrikersToDefault();
    }
}