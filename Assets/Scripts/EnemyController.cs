using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyBase EnemyPrefab;
    public float ShotDelay;

    private List<EnemyBase> spawnedEnemies = new List<EnemyBase>();
    private List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    private bool enemySpawnInitialized;
    private float spawnStartDelay = 0;
    private float spawnInterval = 0.7f;
    private static EnemyController _instance;
    public static EnemyController Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<EnemyController>();
            }
            return _instance;
        }
    }

    private bool allEnemySpawned;

    public int EnemyCount { get { return spawnedEnemies.Count; } }
    public int KilledEnemyCounter { get; private set; }

    public void TryInitEnemySpawning()
    {
        StartCoroutine(EnemySpawnRoutine());
    }

    private IEnumerator EnemySpawnRoutine()
    {
        List<EnemySpawner> spawners = new List<EnemySpawner>(enemySpawners);
        spawners.Shuffle();
        yield return new WaitForSeconds(spawnStartDelay);
        for (int i = 0; i < spawners.Count; i++)
        {
            yield return new WaitForSeconds(spawnInterval);
            EnemyBase enemy = spawners[i].TrySpawn(EnemyPrefab);
            if (enemy != null)
            {
                spawnedEnemies.Add(enemy);
                GameController.Instance.CallOnGameStateChanged();
            }
        }
        allEnemySpawned = true;
    }

    public void UnregisterEnemy(EnemyBase enemy)
    {
        if (!GameController.GameEnded)
        {
            spawnedEnemies.Remove(enemy);
            KilledEnemyCounter++;
            GameController.Instance.CallOnGameStateChanged();
            if (allEnemySpawned && spawnedEnemies.Count == 0)
            {
                GameController.Instance.EndGameWin();
            }
        }
    }

    public void RegisterSpawner(EnemySpawner spawner)
    {
        enemySpawners.Add(spawner);
    }

    private void OnLevelWasLoaded(int level)
    {
        _instance = null;
    }
}