using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light Light;
    public Ship Ship;

    private void Update()
    {
        Light.transform.LookAt(Ship.transform);
    }
}