using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;
using System;

public class PlayersManager : MonoBehaviour
{
    public List<PlayerInfo> AllPlayersData = new List<PlayerInfo>();
    public List<PlayerInfo> ActivePlayers = new List<PlayerInfo>();

    private static bool initialized = false;

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
    }


    private static PlayersManager _instance;
    public static PlayersManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayersManager>();
            }
            return _instance;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            TryAddNewKeyboardPlayer();
        }

        if(GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Index.One))
        {
            TryAddGamepadPlayer(GamePad.Index.One);
        }

        if (GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Index.Two))
        {
            TryAddGamepadPlayer(GamePad.Index.Two);
        }

        if (GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Index.Three))
        {
            TryAddGamepadPlayer(GamePad.Index.Three);
        }

        if (GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Index.Four))
        {
            TryAddGamepadPlayer(GamePad.Index.Four);
        }
    }

    private void TryAddGamepadPlayer(GamePad.Index index)
    {
        if (!IsPlayerWithPadActive(index))
        {
            CreateNewGamepadPlayer(index);
        }
    }

    private void CreateNewGamepadPlayer(GamePad.Index index)
    {
        PlayerInfo playerInfo = GetPlayerDataForGamepad(index);
        if(playerInfo != null)
        {
            PlayerInfo newPlayer = new PlayerInfo(playerInfo);
            ActivePlayers.Add(newPlayer);
            if (GameController.Instance != null)
            {
                GameController.Instance.CreatePlayer(newPlayer);
            }
        }
    }

    public bool IsPlayerWithPadActive(GamePad.Index index)
    {
        for (int i = 0; i < ActivePlayers.Count; i++)
        {
            PlayerInfo player = ActivePlayers[i];
            if (player.GamepadIndex == index)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsKeyboardPlayerActive()
    {
        for (int i = 0; i < ActivePlayers.Count; i++)
        {
            PlayerInfo player = ActivePlayers[i];
            if (player.IsKeyboardAndMouse)
            {
                return true;
            }
        }
        return false;
    }

    private void TryAddNewKeyboardPlayer()
    {
        if (IsKeyboardPlayerActive())
        {
            return;
        }
        PlayerInfo playerInfo = AllPlayersData.Find(x => x.IsKeyboardAndMouse);
        if (playerInfo != null)
        {
            PlayerInfo newPlayer = new PlayerInfo(playerInfo);
            ActivePlayers.Add(newPlayer);
            if (GameController.Instance != null)
            {
                GameController.Instance.CreatePlayer(newPlayer);
            }
        }
    }

    private PlayerInfo GetPlayerDataForGamepad(GamePad.Index index)
    {
        PlayerInfo result = AllPlayersData.Find(x => x.GamepadIndex == index);
        return result;
    }
}