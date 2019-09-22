using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public BallPool BallPool;
    public Transform BallSpawnPosition;

    public int BallCountInPlay = 0;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
        }
    }
}