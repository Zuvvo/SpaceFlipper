﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour
{
    public HingeJoint2D HingeJoint;
    public StrikerPivotType StrikerType;
    public StrikerState StrikerState;

    public SpriteRenderer SpriteRenderer;
    public Color ForceColor;
    public Color DefaultColor;


    private bool isMovingOrMovedUp;
    private bool isForceModeOn;

    private JointAngleLimits2D angleLimitsLeftStriker = new JointAngleLimits2D() { min = -45, max = 45 };
    private JointAngleLimits2D angleLimitsRightStriker = new JointAngleLimits2D() { min = 45, max = -45 };

    private JointAngleLimits2D dodgeLimitsLeftStriker = new JointAngleLimits2D() { min = -115, max = -115 };
    private JointAngleLimits2D dodgeLimitsRightStriker = new JointAngleLimits2D() { min = -245, max = -245 };

    private JointMotor2D motorSpeedUp = new JointMotor2D() { maxMotorTorque = 10000, motorSpeed = 2000 };
    private JointMotor2D motorSpeedDown = new JointMotor2D() { maxMotorTorque = 10000, motorSpeed = -2000 };

    private Vector3 leftStrikerPowerHitForce = new Vector3(2.7f, 0, 1.35f);
    private Vector3 rightStrikerPowerHitForce = new Vector3(2.7f, 0, -1.35f);

    private Vector3 leftStrikerIdleHitForce = new Vector3(1.8f, 0, 0.9f);
    private Vector3 rightStrikerIdleHitForce = new Vector3(1.8f, 0, -0.9f);

    private Vector3 leftStrikerMovedUpHitForce = new Vector3(1.8f, 0, -0.9f);
    private Vector3 rightStrikerMovedUpHitForce = new Vector3(1.8f, 0, 0.9f);

    private Coroutine forceModeRoutine;
    private float forceModeDelay = 0.3f;

    private void Start()
    {
        SetStrikerToDefaultState();
    }

    #region collisions
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.collider.GetComponent<BallBase>();
            if (ball != null)
            {
                AddForceBasedOnHitStrikerState(ball);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.collider.GetComponent<BallBase>();
            if (ball != null)
            {
                ball.SetLastFrameVelocityOnCollisionStay();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.collider.GetComponent<BallBase>();
            if (ball != null)
            {
                ball.SetLastFrameVelocityOnCollisionExit();
            }
        }
    }
    #endregion

    public void MoveBlade()
    {
        Debug.Log("MoveBlade");
        isMovingOrMovedUp = true;
        HingeJoint.useMotor = true;
        HingeJoint.motor = StrikerType == StrikerPivotType.Left ? motorSpeedUp : motorSpeedDown;
        SetForceModeOn();
    }

    public void StopBlade()
    {
        // HingeJoint.motor = StrikerType == StrikerPivotType.Left ? motorSpeedDown : motorSpeedUp;
        HingeJoint.useMotor = false;
        isMovingOrMovedUp = false;
    }

    private void SetForceModeOn()
    {
        SetForceModeOff();
        isForceModeOn = true;
        SpriteRenderer.color = ForceColor;
        forceModeRoutine = StartCoroutine(ForceModeDelayRoutine());
    }

    private void SetForceModeOff()
    {
        if(forceModeRoutine != null)
        {
            StopCoroutine(forceModeRoutine);
            forceModeRoutine = null;
        }
        SpriteRenderer.color = DefaultColor;
        isForceModeOn = false;
    }

    private IEnumerator ForceModeDelayRoutine()
    {
        yield return new WaitForSeconds(forceModeDelay);
        SetForceModeOff();
    }

    private void AddForceBasedOnHitStrikerState(BallBase ball)
    {
        if (isForceModeOn)
        {
            ball.AddForceOnStrikerHit(StrikerType == StrikerPivotType.Left ? leftStrikerPowerHitForce : rightStrikerPowerHitForce, PhysicsConstants.BallSpeedAfterStrikerForceHit);
        }
        else
        {
            if (isMovingOrMovedUp)
            {
                ball.AddForceOnStrikerHit(StrikerType == StrikerPivotType.Left ? leftStrikerMovedUpHitForce : rightStrikerMovedUpHitForce, PhysicsConstants.BallSpeedAfterStrikerIdleHit);
            }
            else
            {
                ball.AddForceOnStrikerHit(StrikerType == StrikerPivotType.Left ? leftStrikerIdleHitForce : rightStrikerIdleHitForce, PhysicsConstants.BallSpeedAfterStrikerIdleHit);
            }
        }
    }

    #region special moves
    public void SetStrikerDown()
    {
        StrikerState = StrikerState.Dodge;
        HingeJoint.limits = StrikerType == StrikerPivotType.Left ? dodgeLimitsLeftStriker : dodgeLimitsRightStriker;
    }

    public void SetStrikerToDefaultState()
    {
        StrikerState = StrikerState.Default;
        HingeJoint.limits = StrikerType == StrikerPivotType.Left ? angleLimitsLeftStriker : angleLimitsRightStriker;
    }

    #endregion
}