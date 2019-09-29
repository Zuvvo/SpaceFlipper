﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDebug : MonoBehaviour
{
    public Text DebugText;

    private BallPool ballPool;
    private Ship ship;
    private EnemyController enemyController;

    private GameController gameController;
    
    public void Init(BallPool ballPool, Ship ship, EnemyController enemyController)
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
        gameController.OnGameStateChanged -= RefreshUi;
    }

    private void RefreshUi()
    {
        DebugText.text = string.Format(" Health: {0}\n Enemies: {1}\n Balls: {2}/{3}", ship.Health, enemyController.EnemyCount, GameController.Instance.BallCountInPlay, ballPool.BallsCount);
    }
}