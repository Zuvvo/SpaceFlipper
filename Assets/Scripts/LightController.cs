using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light Light;
    public PlayerShip Ship;

    private float height = 4.5f;

    private bool initialized;

    private void Update()
    {
        if (initialized)
        {
            Light.transform.position = Ship.transform.position + new Vector3(0, height, 0);
        }
    }

    public void Init(PlayerShip ship, Light light)
    {
        Ship = ship;
        Light = light;
        initialized = true;
    }
}