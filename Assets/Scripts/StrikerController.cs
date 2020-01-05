using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class StrikerController : MonoBehaviour
{
    public KeyCode[] StrikerKeyboardInputs;
    public Striker[] Strikers;


    private float gamepadTriggerSensitivity = 0.5f;

    private bool leftTriggerUp;
    private bool rightTriggerUp;

    public void Update()
    {
        GetInputs();
    }

    private void GetInputs()
    {
        if (!GamepadDetector.IsControllerConnected)
        {
            TriggerKeyboardAndMouse();
        }
        else
        {
            TriggerGamepad();
        }
    }

    private void TriggerKeyboardAndMouse()
    {
        for (int i = 0; i < Strikers.Length; i++)
        {
            if (Strikers[0].StrikerState == StrikerState.Default && (Input.GetKeyDown(StrikerKeyboardInputs[i]) || Input.GetMouseButtonDown(i)))
            {
                Strikers[i].MoveBlade();
            }
            else if (Strikers[0].StrikerState == StrikerState.Default && (Input.GetKeyUp(StrikerKeyboardInputs[i]) || Input.GetMouseButtonUp(i)))
            {
                Strikers[i].StopBlade();
            }
        }
    }

    private void TriggerGamepad()
    {
        float leftTrigger = GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, GamePad.Index.Any);
        float rightTrigger = GamePad.GetTrigger(GamePad.Trigger.RightTrigger, GamePad.Index.Any);

        if (Strikers[0].StrikerState == StrikerState.Default)
        {
            TryUseStriker(GamePad.Trigger.LeftTrigger, Strikers[0], ref leftTriggerUp);
        }
        else
        {
            Strikers[0].StopBlade();
        }

        if(Strikers[0].StrikerState == StrikerState.Default)
        {
            TryUseStriker(GamePad.Trigger.RightTrigger, Strikers[1], ref rightTriggerUp);
        }
        else
        {
            Strikers[1].StopBlade();
        }
    }

    private void TryUseStriker(GamePad.Trigger trigger, Striker striker, ref bool triggerState)
    {
        float axis = GamePad.GetTrigger(trigger, GamePad.Index.Any);
        if(!triggerState)
        {
            if(axis >= gamepadTriggerSensitivity)
            {
                triggerState = true;
                striker.MoveBlade();
            }
        }
        else
        {
            if (axis < gamepadTriggerSensitivity)
            {
                triggerState = false;
                striker.StopBlade();
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