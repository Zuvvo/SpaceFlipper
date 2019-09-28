using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour
{
    public HingeJoint HingeJoint;
    public StrikerPivotType StrikerType;
    public StrikerState StrikerState;

    public MeshRenderer MeshRenderer;
    public Material ForceMaterial;
    public Material DefaultMaterial;


    private bool isMovingOrMovedUp;
    private bool isForceModeOn;

    [SerializeField] private float speed = 15000f;
    [SerializeField] private float pressedPosition = 45f;
    [SerializeField] private float dodgePosition = -90f;
    private float damper = 150f;
    private float restPosition = 0f;

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
        HingeJoint.useSpring = true;
    }

    private void Update()
    {
        JointSpring spring = new JointSpring();
        spring.spring = speed;
        spring.damper = damper;


        if (StrikerState == StrikerState.Default)
        {
            float pos = StrikerType == StrikerPivotType.Left ? pressedPosition : pressedPosition * -1;
            spring.targetPosition = isMovingOrMovedUp ? pos : restPosition;
        }
        else if(StrikerState == StrikerState.Dodge)
        {
            spring.targetPosition = StrikerType == StrikerPivotType.Left ? dodgePosition : dodgePosition * -1;
        }

        HingeJoint.spring = spring;
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
        isMovingOrMovedUp = true;
        SetForceModeOn();
    }

    public void StopBlade()
    {
        isMovingOrMovedUp = false;
    }

    private void SetForceModeOn()
    {
        SetForceModeOff();
        isForceModeOn = true;
        MeshRenderer.material = ForceMaterial;
        forceModeRoutine = StartCoroutine(ForceModeDelayRoutine());
    }

    private void SetForceModeOff()
    {
        if(forceModeRoutine != null)
        {
            StopCoroutine(forceModeRoutine);
            forceModeRoutine = null;
        }
        MeshRenderer.material = DefaultMaterial;
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
    }

    public void SetStrikerToDefaultState()
    {
        StrikerState = StrikerState.Default;
    }

    #endregion
}