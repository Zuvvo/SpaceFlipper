using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public BallBase BallToSpawn;
    public KeyCode TeleportBallAtSpawnPoint;
    public KeyCode SpawnBallAtRandomPoint;
    public Transform BallTestSpawnPosition;

    public void Update()
    {
        if (Input.GetKeyDown(TeleportBallAtSpawnPoint))
        {
            ObjectsController.Instance.TestBallBase.Stop();
            ObjectsController.Instance.TestBallBase.transform.position = BallTestSpawnPosition.position;
        }

        if (Input.GetKeyDown(SpawnBallAtRandomPoint))
        {
            Vector3 pos = new Vector3(BallTestSpawnPosition.position.x, BallTestSpawnPosition.position.y, BallTestSpawnPosition.position.z + Random.Range(0, 2f));
            BallToSpawn = Instantiate(BallToSpawn, pos, Quaternion.identity, null);
        }
    }
}