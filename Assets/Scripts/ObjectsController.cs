using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsController : MonoBehaviour
{
    public BallBase TestBallBase;

    private static ObjectsController _instance;
    public static ObjectsController Instance
    {
        get
        {
            if(_instance == null)
            {
                return FindObjectOfType<ObjectsController>();
            }
            return _instance;
        }
    }

}