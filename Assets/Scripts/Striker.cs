using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour, IRayCollider
{
    public HingeJoint2D HingeJoint;
    public StrikerPivotType StrikerType;
    public StrikerState StrikerState;
    public CollisionType CollisionType;

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

    private Coroutine forceModeRoutine;
    private float forceModeDelay = 0.25f;

    private void Start()
    {
        SetStrikerToDefaultState();
        RegisterObject();
    }

    private void OnDestroy()
    {
        Unregister();
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

    public CollisionSide GetCollisionSideWithBall(Vector2 centroidPoint)
    {
        CollisionSide colSide = CollisionSideDetect.GetCollisionSideBasedOnTriangleAndBottomPoint(LeftPoint.position, RightPoint.position, BottomPoint.position, centroidPoint);
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
        Vector2 forceVector = StrikerCollisionForceManager.GetCollisionEndVelocity(CollisionType, CollisionSide.Bottom, isMovingOrMovedUp, false);
        float endSpeed = PhysicsConstants.BallSpeedAfterStrikerIdleHit;
        return (ball.LastFrameVelocity.normalized + forceVector).normalized * endSpeed;
    }

    private Vector2 GetForceOnBottomSide(BallBase ball)
    {
        Vector2 forceVector = StrikerCollisionForceManager.GetCollisionEndVelocity(CollisionType, CollisionSide.Top, isMovingOrMovedUp, isForceModeOn);
        float endSpeed = isForceModeOn ? PhysicsConstants.BallSpeedAfterStrikerForceHit : PhysicsConstants.BallSpeedAfterStrikerIdleHit;
        return (ball.LastFrameVelocity.normalized + forceVector).normalized * endSpeed;
    }

    private Vector2 GetForceOnLeftOrRight(BallBase ball, CollisionSide colSide)
    {
        Vector2 forceVector = StrikerCollisionForceManager.GetCollisionEndVelocity(CollisionType, colSide, isMovingOrMovedUp, isForceModeOn);
        float endSpeed = PhysicsConstants.BallSpeedAfterStrikerIdleHit;
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

    #region IRayCollider
    public void Raycast()
    {

    }

    public void RegisterObject()
    {
        RayCollidersController.Instance.RegisterRayCollider(this, addAsFirst: true);
    }

    public void Unregister()
    {
        if(RayCollidersController.Instance != null)
        {
            RayCollidersController.Instance.UnregisterRayCollider(this);
        }
    }

    public void OnFixedUpdateTick()
    {

    }

    public List<IRayCollider> HandleCollision(List<IRayCollider> collidersToSkip)
    {
        throw new NotImplementedException();
    }

    public void RegisterCollision(RaycastHit2D rayHit)
    {

    }
    #endregion
}