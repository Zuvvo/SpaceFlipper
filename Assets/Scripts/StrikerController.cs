using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerController : MonoBehaviour
{
    public KeyCode[] StrikerInputs;

    public Striker[] Strikers;

    public void Update()
    {
        GetInputs();
    }

    private void GetInputs()
    {
        for (int i = 0; i < Strikers.Length; i++)
        {
            if (Input.GetKey(StrikerInputs[i]))
            {
                Strikers[i].MoveBlade();
            }
            else
            {
                Strikers[i].StopBlade();
            }
        }
    }

    private static BallBase _ballBase;
    public static BallBase BallBase
    {
        get
        {
            if(_ballBase == null)
            {
                _ballBase = FindObjectOfType<BallBase>();
            }
            return _ballBase;
        }
    }
}