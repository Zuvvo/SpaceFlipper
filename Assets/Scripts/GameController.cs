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
    
    public ShipController ShipController;
    public UiDebug UiDebug;
    public UiFinishGameInfo UiFinishGameInfo;

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

    private void Start()
    {
        UiDebug.Init(BallPool, ShipController.Ship, EnemyController.Instance);
    }

    private void Update()
    {
        bool isKeyDown = GamepadDetector.IsControllerConnected ? GamePad.GetButtonDown(GamePad.Button.Y, GamePad.Index.Any) : Input.GetKeyDown(KeyCode.Space);
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

    public void EndGameLose()
    {
        GameEnded = true;
        Time.timeScale = 0;
        UiFinishGameInfo.Init(false);
    }

    private void OnLevelWasLoaded(int level)
    {
        _instance = null;
    }
}