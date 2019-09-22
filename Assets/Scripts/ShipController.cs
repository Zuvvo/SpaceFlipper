using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public Ship Ship;
    public float Speed = 20;

    private void Update()
    {
        float dirHorizontal = Input.GetAxisRaw("Horizontal");
        float dirVertical = Input.GetAxisRaw("Vertical");
        Ship.RigidBody.velocity = new Vector3(dirVertical * (Speed / 2.5f), 0, -dirHorizontal * Speed);
    }
}