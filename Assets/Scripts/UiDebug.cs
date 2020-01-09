using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDebug : MonoBehaviour
{
    public Text DebugText;

    private BallPool ballPool;
    private PlayerShip ship;
    private EnemyController enemyController;

    private GameController gameController;
    
    public void Init(BallPool ballPool, PlayerShip ship, EnemyController enemyController)
    {
        this.ballPool = ballPool;
        this.ship = ship;
        this.enemyController = enemyController;
        gameController = GameController.Instance;
        gameController.OnGameStateChanged += RefreshUi;
        RefreshUi();
    }

    private void OnDestroy()
    {
        if(gameController != null)
        {
            gameController.OnGameStateChanged -= RefreshUi;
        }
    }

    private void RefreshUi()
    {
        DebugText.text = string.Format(" Health: {0}\n Enemies: {1}\n Balls: {2}/{3}", ship.Health, enemyController.EnemyCount, GameController.Instance.BallsInPlay.Count, ballPool.BallsCount);
    }
}