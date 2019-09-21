using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour
{
    public HingeJoint HingeJoint;
    public StrikerPivotType StrikerType;

    public MeshRenderer MeshRenderer;
    public Material ForceMaterial;
    public Material DefaultMaterial;


    private bool isMovingOrMovedUp;
    private bool isForceModeOn;

    [SerializeField] private float speed = 15000f;
    [SerializeField] private float pressedPosition = 45f;
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


        float pos = StrikerType == StrikerPivotType.Left ? pressedPosition : pressedPosition * -1;
        spring.targetPosition = isMovingOrMovedUp ? pos : restPosition;

        HingeJoint.spring = spring;
        HingeJoint.useSpring = true;
    }
   
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
            ball.AddForceOnStrikerHit(StrikerType == StrikerPivotType.Left ? leftStrikerPowerHitForce : rightStrikerPowerHitForce);
        }
        else
        {
            if (isMovingOrMovedUp)
            {
                ball.AddForceOnStrikerHit(StrikerType == StrikerPivotType.Left ? leftStrikerMovedUpHitForce : rightStrikerMovedUpHitForce);
            }
            else
            {
                ball.AddForceOnStrikerHit(StrikerType == StrikerPivotType.Left ? leftStrikerIdleHitForce : rightStrikerIdleHitForce);
            }
        }
    }
}