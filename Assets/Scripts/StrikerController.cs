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
            if (Input.GetKeyDown(StrikerInputs[i]) || Input.GetMouseButtonDown(i))
            {
                Strikers[i].MoveBlade();
            }
            else if(Input.GetKeyUp(StrikerInputs[i]) || Input.GetMouseButtonUp(i))
            {
                Strikers[i].StopBlade();
            }
        }
    }

    public void OnDodge()
    {
        for (int i = 0; i < Strikers.Length; i++)
        {
            Strikers[i].SetStrikerDown();
        }
    }
    
    public void SetStrikersToDefault()
    {
        for (int i = 0; i < Strikers.Length; i++)
        {
            Strikers[i].SetStrikerToDefaultState();
        }
    }
}