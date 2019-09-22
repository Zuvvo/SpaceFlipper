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

    private float spawnStartDelay = 2;
    private float spawnInterval = 0.2f;
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

    private void Start()
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
            }
        }
    }

    public void UnregisterEnemy(EnemyBase enemy)
    {
        spawnedEnemies.Remove(enemy);
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