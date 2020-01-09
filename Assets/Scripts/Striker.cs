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
    private JointAngleLimits2D dodgeLimitsRightStriker = new JointAngleLimits2D() { min = -245, max = -245 };

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
    private float forceModeDelay = 0.3f;

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

    #region Collisions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.gameObject.GetComponent<BallBase>();
            if (ball != null)
            {
                AddForceBasedOnHitStrikerState(ball);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.gameObject.GetComponent<BallBase>();
            if (ball != null)
            {
                ball.SetLastFrameVelocityOnCollisionStay();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameTags.Ball))
        {
            BallBase ball = collision.gameObject.GetComponent<BallBase>();
            if (ball != null)
            {
                ball.SetLastFrameVelocityOnCollisionExit();
            }
        }
    }
    #endregion

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

    #region Add force to ball
    private void AddForceBasedOnHitStrikerState(BallBase ball)
    {
        CollisionSide colSide = CollisionSideDetect.GetCollisionSideBasedOnTriangleAndBottomPoint(LastFrameLeftPoint, LastFrameRightPoint, LastFrameBottomPoint, ball.LastFrameCenterPoint);

        switch (colSide)
        {
            case CollisionSide.Top:
                AddForceOnTopSide(ball);
                break;
            case CollisionSide.Bottom:
                AddForceOnBottomSide(ball);
                break;
            case CollisionSide.Left:
                ball.AddForceOnStrikerHit(isMovingOrMovedUp ? leftSideMovedUpHitForce : leftSideIdleHitForce, PhysicsConstants.BallSpeedAfterStrikerIdleHit);
                break;
            case CollisionSide.Right:
                ball.AddForceOnStrikerHit(isMovingOrMovedUp ? rightSideMovedUpHitForce : rightSideIdleHitForce, PhysicsConstants.BallSpeedAfterStrikerIdleHit);
                break;
        }
    }

    private void AddForceOnTopSide(BallBase ball)
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

    private void AddForceOnBottomSide(BallBase ball)
    {
        if (isMovingOrMovedUp)
        {
            ball.AddForceOnStrikerHit(StrikerType == StrikerPivotType.Left ? leftBottomMovedUpHitForce : rightBottomMovedUpHitForce, PhysicsConstants.BallSpeedAfterStrikerIdleHit);
        }
        else
        {
            ball.AddForceOnStrikerHit(StrikerType == StrikerPivotType.Left ? leftBottomIdleHitForce : rightBottomIdleHitForce, PhysicsConstants.BallSpeedAfterStrikerIdleHit);
        }
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