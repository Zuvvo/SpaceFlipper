using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public BallBase BallToSpawn;
    public KeyCode TeleportBallAtSpawnPoint;
    public KeyCode SpawnBallAtRandomPoint;
    public Transform BallTestSpawnPosition;

    private int ballCounter;

    private float ballSpwanInterval = 7;
    
    public void Update()
    {
        //if (Input.GetKeyDown(TeleportBallAtSpawnPoint))
        //{
        //    ObjectsController.Instance.TestBallBase.Stop();
        //    ObjectsController.Instance.TestBallBase.transform.position = BallTestSpawnPosition.position;
        //}

        //if (Input.GetKeyDown(SpawnBallAtRandomPoint))
        //{
        //    BallBase ball = SpawnBallOnRandomPoint();
        //    ball.Rigidbody.AddForce(new Vector3(-1, 0, 0), ForceMode.Impulse);
        //}
    }

    private void Start()
    {
       // StartCoroutine(SpawnBallRoutine());
    }

    private IEnumerator SpawnBallRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(ballSpwanInterval);
            SpawnBallOnRandomPoint();
        }
    }

    private BallBase SpawnBallOnRandomPoint()
    {
        Vector3 pos = new Vector3(BallTestSpawnPosition.position.x, BallTestSpawnPosition.position.y, BallTestSpawnPosition.position.z + UnityEngine.Random.Range(0, 2f));
        BallBase ball = Instantiate(BallToSpawn, pos, Quaternion.identity, null);
        ball.gameObject.name = "Ball " + ballCounter;
        ballCounter++;
        return ball;
    }
}