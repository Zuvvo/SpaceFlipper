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
    public Ship Ship;
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
        Ship.RigidBody.velocity = new Vector3(vertical * (speed / 1.5f), 0, -horizontal * (speed));

        bool dodgeKeyDown = GamepadDetector.IsControllerConnected ? GamePad.GetButtonDown(GemepadDodgeButton, GamePad.Index.Any) : Input.GetKeyDown(DodgeKey);

        if (isDodgeReady && dodgeKeyDown && (horizontal != 0 || vertical != 0))
        {
            TryDodge();
        }
    }

    private void GetAxis(out float horizontal, out float vertical)
    {
        if (GamepadDetector.IsControllerConnected)
        {
            Vector2 axis = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Any);
            horizontal = axis.x;
            vertical = axis.y;
        }
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
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