using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour
{
    public HingeJoint HingeJoint;
    public StrikerPivotType StrikerType;


    private bool _moving;

    [SerializeField] private float _speed = 15000f;
    [SerializeField] private float _pressedPosition = 45f;
    private float _damper = 150f;
    private float _restPosition = 0f;

    private void Start()
    {
        HingeJoint.useSpring = true;
    }

    private void Update()
    {
        JointSpring spring = new JointSpring();
        spring.spring = _speed;
        spring.damper = _damper;


        float pos = StrikerType == StrikerPivotType.Left ? _pressedPosition : _pressedPosition * -1;
        spring.targetPosition = _moving ? pos : _restPosition;

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

            }
        }
    }


    public void MoveBlade()
    {
        _moving = true;
    }

    public void StopBlade()
    {
        _moving = false;
    }

}