using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    public Transform BallSpawnPlace;
    public BallBase BallPrefab;

    public int BallLimit = 4;


    private float ballSpawnIntervalMin = 1;
    private float ballSpawnIntervalMax = 1.5f;
    private int ballCounter = 0;
    private List<BallBase> ballsInPool = new List<BallBase>();

    private void Start()
    {
        StartCoroutine(SpawnBallRoutine());
    }

    private IEnumerator SpawnBallRoutine()
    {
        while (true)
        {
            if(ballsInPool.Count + GameController.Instance.BallCountInPlay < BallLimit)
            {
                AddBallToPool();
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(ballSpawnIntervalMin, ballSpawnIntervalMax));
        }
    }

    public BallBase TryTakeBallToPlay()
    {
        if(ballsInPool.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, ballsInPool.Count);
            BallBase randomBall = ballsInPool[randomIndex];
            ballsInPool.RemoveAt(randomIndex);
            return randomBall;
        }
        return null;
    }

    private void AddBallToPool()
    {
        BallBase ball = Instantiate(BallPrefab, BallSpawnPlace.position, Quaternion.identity);
        ball.SetGravityState(true);
        ballsInPool.Add(ball);
        ball.gameObject.name = "Ball " + ballCounter;
        ballCounter++;
    }
}