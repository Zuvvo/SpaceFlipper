using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadDetector : MonoBehaviour
{
    public static bool IsControllerConnected;
    public static event Action<bool> OnControllerConnectedStatusChanged;
    private static bool initialized = false;


    private WaitForSecondsRealtime _waitTime = new WaitForSecondsRealtime(2);
    private string[] joyStickNames;

    private void Awake()
    {
        if (!initialized)
        {
            initialized = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(CheckForController());
        CallOnControllerConnectedStatusChanged();
    }

    private IEnumerator CheckForController()
    {
        while (true)
        {
            joyStickNames = Input.GetJoystickNames();
            for (int i = 0; i < joyStickNames.Length; i++)
            {
                if (!string.IsNullOrEmpty(joyStickNames[i]))
                {
                    if (!IsControllerConnected)
                    {
                        IsControllerConnected = true;
                        CallOnControllerConnectedStatusChanged();
                    }
                }
                else
                {
                    if (IsControllerConnected)
                    {
                        IsControllerConnected = false;
                        CallOnControllerConnectedStatusChanged();
                    }
                }
            }
            yield return _waitTime;
        }
    }

    private void CallOnControllerConnectedStatusChanged()
    {
        OnControllerConnectedStatusChanged?.Invoke(IsControllerConnected);
    }
}