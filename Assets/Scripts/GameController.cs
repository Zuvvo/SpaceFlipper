using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public BallPool BallPool;
    public Transform BallSpawnPosition;
    public PlayerSpawner PlayerSpawner;
    public UiDebug UiDebug;
    public UiFinishGameInfo UiFinishGameInfo;

    public List<PlayerShip> ActivePlayerShips = new List<PlayerShip>();

    public int BallCountInPlay = 0;
    public event Action OnGameStateChanged;
    public bool GameStarted { get; private set; }
    public float GameTime { get; private set; }

    public static bool GameEnded;

    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        GameEnded = false;
    }

    private void Update()
    {
        bool isKeyDown = GamepadDetector.IsAnyControllerConnected ? GamePad.GetButtonDown(GamePad.Button.Y, GamePad.Index.Any) : Input.GetKeyDown(KeyCode.Space);
        if (isKeyDown)
        {
            BallBase ball = BallPool.TryTakeBallToPlay();
            if(ball != null)
            {
                Vector3 pos = new Vector3(BallSpawnPosition.position.x, BallSpawnPosition.position.y, BallSpawnPosition.position.z + UnityEngine.Random.Range(0, 2f));
                ball.SetGravityState(false);
                ball.transform.position = pos;
                ball.Rigidbody.AddForce(new Vector3(-1, 0, 0), ForceMode.Impulse);
                BallCountInPlay++;
            }
            CallOnGameStateChanged();
            GameStarted = true;
            EnemyController.Instance.TryInitEnemySpawning();
        }

        if (GameStarted)
        {
            GameTime += Time.deltaTime;
        }
    }

    public void InitUiForPlayer(ShipController shipController)
    {
        UiDebug.Init(BallPool, shipController.Ship, EnemyController.Instance);
    }

    public void CallOnGameStateChanged()
    {
        OnGameStateChanged?.Invoke();
    }

    public void EndGameWin()
    {
        GameEnded = true;
        Time.timeScale = 0;
        UiFinishGameInfo.Init(true);
    }

    public void CreatePlayer(PlayerInfo playerInfo)
    {
        PlayerShip ship = PlayerSpawner.SpawnPlayer(playerInfo);
        ActivePlayerShips.Add(ship);
    }

    public void EndGameLose()
    {
        GameEnded = true;
        Time.timeScale = 0;
        UiFinishGameInfo.Init(false);
    }

    private void OnLevelWasLoaded(int level)
    {
        _instance = null;

        CreateAllPlayersOnStart();
    }

    private void CreateAllPlayersOnStart()
    {
        List<PlayerInfo> activePlayers = PlayersManager.Instance.ActivePlayers;

        for (int i = 0; i < activePlayers.Count; i++)
        {
            PlayerInfo player = activePlayers[i];
            CreatePlayer(player);
        }
    }
}