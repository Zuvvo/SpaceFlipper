using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GamepadInput;

public class ShipController : MonoBehaviour
{
    public KeyCode DodgeKey = KeyCode.LeftShift;
    public GamePad.Button GemepadDodgeButton = GamePad.Button.B;

    public StrikerController StrikerController;
    public PlayerShip Ship;
    public float Speed = 20;
    public float DodgeSpeed = 100;
    public float DodgeTimeDelay = 2;
    public float DodgeMoveTime = 0.1f;

    private bool isDodgeReady = true;
    public bool IsDodgeMoving { get; private set; }

    private void Update()
    {
        GetAxis(out float horizontal, out float vertical);

        float speed = IsDodgeMoving ? DodgeSpeed : Speed;
        Ship.Rigidbody.velocity = new Vector2(horizontal * (speed), vertical * (speed / 1.5f));

        if (isDodgeReady && IsDodgeKeyDown() && (horizontal != 0 || vertical != 0))
        {
            TryDodge();
        }
    }

    private bool IsDodgeKeyDown()
    {
        if (Ship.PlayerInfo.IsKeyboardAndMouse)
        {
            return Input.GetKeyDown(DodgeKey);
        }
        else
        {
            return GamePad.GetButtonDown(GemepadDodgeButton, Ship.PlayerInfo.GamepadIndex);
        }
    }

    private void GetAxis(out float horizontal, out float vertical)
    {
        if (Ship.PlayerInfo.IsKeyboardAndMouse)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        else
        {
            Vector2 axis = GamePad.GetAxis(GamePad.Axis.LeftStick, Ship.PlayerInfo.GamepadIndex);
            horizontal = axis.x;
            vertical = axis.y;
        }
    }

    private void TryDodge()
    {
        StartCoroutine(DodgeSequence());
    }

    private IEnumerator DodgeSequence()
    {
        StrikerController.OnDodge();
        IsDodgeMoving = true;
        isDodgeReady = false;
        yield return new WaitForSeconds(DodgeMoveTime);
        IsDodgeMoving = false;
        yield return new WaitForSeconds(DodgeTimeDelay - DodgeMoveTime);
        isDodgeReady = true;
        StrikerController.SetStrikersToDefault();
    }
}