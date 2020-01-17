using System;
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

    public Transform LeftPoint;
    public Transform RightPoint;
    public Transform BottomPoint;

    private bool isMovingOrMovedUp;
    private bool isForceModeOn;

    private JointAngleLimits2D angleLimitsLeftStriker = new JointAngleLimits2D() { min = -45, max = 45 };
    private JointAngleLimits2D angleLimitsRightStriker = new JointAngleLimits2D() { min = 45, max = -45 };

    private JointAngleLimits2D dodgeLimitsLeftStriker = new JointAngleLimits2D() { min = -115, max = -115 };
    private JointAngleLimits2D dodgeLimitsRightStriker = new JointAngleLimits2D() { min = 115, max = 115 };

    private JointMotor2D motorSpeedUp = new JointMotor2D() { maxMotorTorque = 10000, motorSpeed = 2000 };
    private JointMotor2D motorSpeedDown = new JointMotor2D() { maxMotorTorque = 10000, motorSpeed = -2000 };

    private Vector2 leftStrikerPowerHitForce = new Vector2(-1.35f, 2.7f);
    private Vector2 rightStrikerPowerHitForce = new Vector2(1.35f, 2.7f);

    private Vector2 leftStrikerIdleHitForce = new Vector2(-0.9f, 1.8f);
    private Vector2 rightStrikerIdleHitForce = new Vector2(0.9f, 1.8f);

    private Vector2 leftStrikerMovedUpHitForce = new Vector2(0.9f, 1.8f);
    private Vector2 rightStrikerMovedUpHitForce = new Vector2(-0.9f, 1.8f);

    private Vector2 leftSideMovedUpHitForce = new Vector2(-1.35f, 1.35f);
    private Vector2 rightSideMovedUpHitForce = new Vector2(1.35f, 1.35f);

    private Vector2 leftSideIdleHitForce = new Vector2(-1.35f, -1.35f);
    private Vector2 rightSideIdleHitForce = new Vector2(1.35f, -1.35f);

    private Vector2 leftBottomIdleHitForce = new Vector2(0.9f, -1.35f);
    private Vector2 leftBottomMovedUpHitForce = new Vector2(-0.9f, -1.35f);

    private Vector2 rightBottomIdleHitForce = new Vector2(-0.9f, -1.35f);
    private Vector2 rightBottomMovedUpHitForce = new Vector2(0.9f, -1.35f);

    private Coroutine forceModeRoutine;
    private float forceModeDelay = 0.25f;

    public Vector2 LastFrameLeftPoint { get; private set; }
    public Vector2 LastFrameRightPoint { get; private set; }
    public Vector2 LastFrameBottomPoint { get; private set; }

    private void Start()
    {
        SetStrikerToDefaultState();
    }

    private void Update()
    {
        LastFrameLeftPoint = LeftPoint.position;
        LastFrameRightPoint = RightPoint.position;
        LastFrameBottomPoint = BottomPoint.position;
    }

    #region Blade move
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
    #endregion

    public CollisionSide GetCollisionSideWithBall(BallBase ball, Vector2 centroidPoint)
    {
        CollisionSide colSide = CollisionSideDetect.GetCollisionSideBasedOnTriangleAndBottomPoint(LastFrameLeftPoint, LastFrameRightPoint, LastFrameBottomPoint, centroidPoint);
        Debug.Log("Striker col: " + colSide);
        return colSide;
    }

    public Vector2 GetForceOnBallHit(BallBase ball, CollisionSide colSide)
    {
        switch (colSide)
        {
            case CollisionSide.Top:
                return GetForceOnTopSide(ball);
            case CollisionSide.Bottom:
                return GetForceOnBottomSide(ball);
            case CollisionSide.Left:
            case CollisionSide.Right:
                return GetForceOnLeftOrRight(ball, colSide);
            default:
                return Vector2.one;
        }
    }

    #region Get force on ball hit

    private Vector2 GetForceOnTopSide(BallBase ball)
    {
        Vector2 forceVector = new Vector2();
        float endSpeed = 0;
        if (isForceModeOn)
        {
            endSpeed = PhysicsConstants.BallSpeedAfterStrikerForceHit;
            forceVector = StrikerType == StrikerPivotType.Left ? leftStrikerPowerHitForce : rightStrikerPowerHitForce;
        }
        else
        {
            endSpeed = PhysicsConstants.BallSpeedAfterStrikerIdleHit;
            if (isMovingOrMovedUp)
            {
                forceVector = StrikerType == StrikerPivotType.Left ? leftStrikerMovedUpHitForce : rightStrikerMovedUpHitForce;
            }
            else
            {
                forceVector = StrikerType == StrikerPivotType.Left ? leftStrikerIdleHitForce : rightStrikerIdleHitForce;
            }
        }

        return (ball.LastFrameVelocity.normalized + forceVector).normalized * endSpeed;
    }

    private Vector2 GetForceOnBottomSide(BallBase ball)
    {
        Vector2 forceVector = new Vector2();
        float endSpeed = PhysicsConstants.BallSpeedAfterStrikerIdleHit;
        if (isMovingOrMovedUp)
        {
            forceVector = StrikerType == StrikerPivotType.Left ? leftBottomMovedUpHitForce : rightBottomMovedUpHitForce;
        }
        else
        {
            forceVector = StrikerType == StrikerPivotType.Left ? leftBottomIdleHitForce : rightBottomIdleHitForce;
        }

        return (ball.LastFrameVelocity.normalized + forceVector).normalized * endSpeed;
    }

    private Vector2 GetForceOnLeftOrRight(BallBase ball, CollisionSide colSide)
    {
        Vector2 forceVector = new Vector2();
        float endSpeed = PhysicsConstants.BallSpeedAfterStrikerIdleHit;

        if (colSide == CollisionSide.Left)
        {
            forceVector = isMovingOrMovedUp ? leftSideMovedUpHitForce : leftSideIdleHitForce;
        }
        else if(colSide == CollisionSide.Right)
        {
            forceVector = isMovingOrMovedUp ? rightSideMovedUpHitForce : rightSideIdleHitForce;
        }

        return (ball.LastFrameVelocity.normalized + forceVector).normalized * endSpeed;
    }
    #endregion

    #region Special moves
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