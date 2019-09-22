using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyBase EnemyPrefab;
    public float SpawnDelay;
    public float ShotDelay;

    private List<EnemyBase> spawnedEnemies = new List<EnemyBase>();
    private List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

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
        StartCoroutine(EnemyShotRoutine());
    }

    private IEnumerator EnemyShotRoutine()
    {
        while (true)
        {
            if(spawnedEnemies.Count > 0)
            {
                EnemyBase randomEnemy = spawnedEnemies[UnityEngine.Random.Range(0, spawnedEnemies.Count - 1)];
                randomEnemy.Shoot();
            }
            yield return new WaitForSeconds(ShotDelay);
        }
    }

    private IEnumerator EnemySpawnRoutine()
    {
        while (true)
        {
            List<EnemySpawner> notOccupiedSpawners = new List<EnemySpawner>(enemySpawners.FindAll(x => !x.IsOccupied));
            if (notOccupiedSpawners.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, notOccupiedSpawners.Count - 1);
                EnemyBase enemy = notOccupiedSpawners[randomIndex].TrySpawn(EnemyPrefab);
                if(enemy != null)
                {
                    spawnedEnemies.Add(enemy);
                }
            }
            yield return new WaitForSeconds(SpawnDelay);
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