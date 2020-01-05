using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

[System.Serializable]
public class PlayerInfo
{
    public string Name = "PlayerName";
    public bool IsKeyboardAndMouse;
    public GamePad.Index GamepadIndex = GamePad.Index.Any;


    public PlayerInfo() { }

    public PlayerInfo(PlayerInfo source)
    {
        Name = source.Name;
        IsKeyboardAndMouse = source.IsKeyboardAndMouse;
        GamepadIndex = source.GamepadIndex;
    }
}